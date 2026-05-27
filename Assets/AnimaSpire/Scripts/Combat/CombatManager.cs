using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageDifficultyCalculator stageDifficultyCalculator;
    [SerializeField] private HeroUnit hero;
    [SerializeField] private SpiritUnit spirit;
    [SerializeField] private EnemyUnit enemy;
    [Header("Runtime Start Positions")]
    [SerializeField] private Vector3 heroStartLocalPosition;
    [SerializeField] private Vector3 spiritStartLocalPosition;
    [SerializeField] private Vector3 enemyStartLocalPosition;
    [Header("Enemy Runtime Movement")]
    [SerializeField] private float enemyMoveSpeed = 0.75f;
    [SerializeField] private float enemyStopDistance = 1.25f;
    [Header("Attack Ranges")]
    [SerializeField] private float heroAttackRange = 3.0f;
    [SerializeField] private float enemyAttackRange = 1.5f;
    [Header("Hero Projectile")]
    [SerializeField] private int heroProjectileInitialPoolSize = 10;
    [SerializeField] private float heroProjectileSpeed = 6f;
    [SerializeField] private float heroProjectileHitDistance = 0.1f;
    [SerializeField] private float heroProjectileVisualSize = 0.16f;
    [SerializeField] private Color heroProjectileColor = new Color(0.35f, 0.85f, 1f, 1f);

    private float heroAttackTimer;
    private float spiritAttackTimer;
    private float enemyAttackTimer;
    private bool isResolvingCombat;
    private int currentStageToken;
    private readonly Queue<HeroProjectile> inactiveHeroProjectiles = new Queue<HeroProjectile>();
    private readonly List<HeroProjectile> activeHeroProjectiles = new List<HeroProjectile>();
    private Transform heroProjectileRoot;
    private Sprite heroProjectileSprite;

    private void OnValidate()
    {
        ValidateAttackRangeSettings();
    }

    private void Start()
    {
        EnsureReferences();
        ValidateAttackRangeSettings();
        EnsureHeroProjectilePool();

        BeginNewCombatRound();
        ResetRuntimePositionsForCombat();
        ApplyCurrentStageEnemyStats();
        ResetAttackTimers();
        Debug.Log("CombatManager initialized.");
    }

    private void Update()
    {
        if (gameManager == null || hero == null || spirit == null || enemy == null)
        {
            return;
        }

        if (isResolvingCombat)
        {
            return;
        }

        if (!enemy.IsAlive)
        {
            StartCoroutine(RewardGoldAndResetCombat());
            return;
        }

        if (!hero.IsAlive)
        {
            StartCoroutine(RetreatStageAndResetCombat());
            return;
        }

        bool enemyMovedThisFrame = UpdateEnemyMovement();

        heroAttackTimer += Time.deltaTime;
        spiritAttackTimer += Time.deltaTime;
        enemyAttackTimer += Time.deltaTime;

        if (heroAttackTimer >= hero.castInterval)
        {
            if (IsWithinRange(hero.transform, enemy.transform, heroAttackRange))
            {
                heroAttackTimer = 0f;
                FireHeroProjectile();
            }
        }

        if (spiritAttackTimer >= spirit.castInterval)
        {
            spiritAttackTimer = 0f;
            spirit.DealDamage(enemy);
        }

        if (enemy.IsAlive && hero.IsAlive && enemyAttackTimer >= enemy.attackInterval)
        {
            if (!enemyMovedThisFrame && IsWithinRange(enemy.transform, hero.transform, enemyAttackRange))
            {
                enemyAttackTimer = 0f;
                enemy.DealDamage(hero);
            }
        }
    }

    private IEnumerator RewardGoldAndResetCombat()
    {
        isResolvingCombat = true;
        ReturnAllActiveHeroProjectiles();
        gameManager.AddGold(CalculateCurrentStageGoldReward());
        stageManager?.AdvanceStage();

        yield return new WaitForSeconds(1f);

        BeginNewCombatRound();
        ResetRuntimePositionsForCombat();
        hero.ResetHp();
        ApplyCurrentStageEnemyStats();
        ResetAttackTimers();
        isResolvingCombat = false;
    }

    private IEnumerator RetreatStageAndResetCombat()
    {
        isResolvingCombat = true;

        string failedStageLabel = stageManager != null ? stageManager.GetCurrentStageLabel() : "Unknown";
        Debug.Log($"Stage failed: {failedStageLabel}");

        stageManager?.RetreatStageOnFailure();
        ReturnAllActiveHeroProjectiles();

        string retreatStageLabel = stageManager != null ? stageManager.GetCurrentStageLabel() : "Unknown";
        Debug.Log($"Retreat to Stage: {retreatStageLabel}");

        yield return new WaitForSeconds(1f);

        BeginNewCombatRound();
        ResetRuntimePositionsForCombat();
        hero.ResetHp();
        ApplyCurrentStageEnemyStats();
        ResetAttackTimers();
        isResolvingCombat = false;
    }

    public void RestartCombatForLoadedProgress()
    {
        EnsureReferences();
        EnsureHeroProjectilePool();
        StopAllCoroutines();
        isResolvingCombat = false;
        BeginNewCombatRound();
        ResetRuntimePositionsForCombat();
        hero?.ResetHp();
        ApplyCurrentStageEnemyStats();
        ResetAttackTimers();
        Debug.Log("Combat restarted for loaded progress.");
    }

    private void ValidateAttackRangeSettings()
    {
        if (enemyAttackRange < enemyStopDistance)
        {
            Debug.LogWarning("enemyAttackRange is smaller than enemyStopDistance. Enemy may move inside the visual stop distance before attacking.");
        }
    }

    private void EnsureReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
        }

        if (stageDifficultyCalculator == null)
        {
            stageDifficultyCalculator = FindAnyObjectByType<StageDifficultyCalculator>();
        }

        if (hero == null)
        {
            hero = FindAnyObjectByType<HeroUnit>();
        }

        if (spirit == null)
        {
            spirit = FindAnyObjectByType<SpiritUnit>();
        }

        if (enemy == null)
        {
            enemy = FindAnyObjectByType<EnemyUnit>();
        }
    }

    private void EnsureHeroProjectilePool()
    {
        if (heroProjectileRoot == null)
        {
            GameObject rootObject = new GameObject("HeroProjectilePool");
            rootObject.transform.SetParent(transform, false);
            heroProjectileRoot = rootObject.transform;
        }

        if (heroProjectileSprite == null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            heroProjectileSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }

        int targetPoolSize = Mathf.Max(heroProjectileInitialPoolSize, 0);
        while (inactiveHeroProjectiles.Count + activeHeroProjectiles.Count < targetPoolSize)
        {
            inactiveHeroProjectiles.Enqueue(CreateHeroProjectile());
        }
    }

    private HeroProjectile CreateHeroProjectile()
    {
        GameObject projectileObject = new GameObject("HeroProjectile");
        projectileObject.transform.SetParent(heroProjectileRoot, false);
        projectileObject.transform.localScale = Vector3.one * Mathf.Max(heroProjectileVisualSize, 0.01f);

        SpriteRenderer spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = heroProjectileSprite;
        spriteRenderer.color = heroProjectileColor;
        spriteRenderer.sortingOrder = 20;

        HeroProjectile projectile = projectileObject.AddComponent<HeroProjectile>();
        projectileObject.SetActive(false);
        return projectile;
    }

    private HeroProjectile GetHeroProjectileFromPool()
    {
        EnsureHeroProjectilePool();

        HeroProjectile projectile = inactiveHeroProjectiles.Count > 0
            ? inactiveHeroProjectiles.Dequeue()
            : CreateHeroProjectile();
        activeHeroProjectiles.Add(projectile);
        return projectile;
    }

    private void ReturnHeroProjectileToPool(HeroProjectile projectile)
    {
        if (projectile == null)
        {
            return;
        }

        activeHeroProjectiles.Remove(projectile);
        inactiveHeroProjectiles.Enqueue(projectile);
    }

    private void ReturnAllActiveHeroProjectiles()
    {
        for (int i = activeHeroProjectiles.Count - 1; i >= 0; i--)
        {
            activeHeroProjectiles[i]?.ForceReturn();
        }
    }

    private void BeginNewCombatRound()
    {
        currentStageToken++;
        ReturnAllActiveHeroProjectiles();
    }

    private void ApplyCurrentStageEnemyStats()
    {
        if (stageManager == null || stageDifficultyCalculator == null || enemy == null)
        {
            enemy?.ResetHp();
            return;
        }

        EnemyStageStats enemyStats = stageDifficultyCalculator.CalculateEnemyStats(
            stageManager.CurrentArea,
            stageManager.CurrentStage,
            stageManager.MaxStagePerArea);
        enemy.ApplyStats(enemyStats.MaxHp, enemyStats.AttackPower);
    }

    private void ResetRuntimePositionsForCombat()
    {
        ResetRuntimeLocalPosition(hero != null ? hero.transform : null, heroStartLocalPosition);
        ResetRuntimeLocalPosition(spirit != null ? spirit.transform : null, spiritStartLocalPosition);
        ResetRuntimeLocalPosition(enemy != null ? enemy.transform : null, enemyStartLocalPosition);
    }

    private static void ResetRuntimeLocalPosition(Transform target, Vector3 startLocalPosition)
    {
        if (target == null)
        {
            return;
        }

        target.localPosition = startLocalPosition;
    }

    private bool UpdateEnemyMovement()
    {
        if (hero == null || enemy == null || !hero.IsAlive || !enemy.IsAlive)
        {
            return false;
        }

        Transform heroTransform = hero.transform;
        Transform enemyTransform = enemy.transform;
        Vector3 heroPosition = heroTransform.position;
        Vector3 enemyPosition = enemyTransform.position;
        float stopDistance = Mathf.Max(enemyAttackRange, 0f);

        if (IsWithinRange(enemyTransform, heroTransform, stopDistance))
        {
            return false;
        }

        Vector3 heroToEnemy = enemyPosition - heroPosition;
        if (heroToEnemy.sqrMagnitude <= Mathf.Epsilon)
        {
            return false;
        }

        Vector3 directionFromHeroToEnemy = heroToEnemy.normalized;
        Vector3 stopPosition = heroPosition + directionFromHeroToEnemy * stopDistance;
        float maxDistanceDelta = Mathf.Max(enemyMoveSpeed, 0f) * Time.deltaTime;
        Vector3 nextPosition = Vector3.MoveTowards(enemyPosition, stopPosition, maxDistanceDelta);
        enemyTransform.position = nextPosition;
        return nextPosition != enemyPosition;
    }

    private void FireHeroProjectile()
    {
        if (hero == null || enemy == null || !hero.IsAlive || !enemy.IsAlive)
        {
            return;
        }

        float damageSnapshot = hero.CalculateAttackDamage();
        HeroProjectile projectile = GetHeroProjectileFromPool();
        projectile.Fire(
            hero.transform.position,
            enemy,
            damageSnapshot,
            currentStageToken,
            heroProjectileSpeed,
            heroProjectileHitDistance,
            GetCurrentStageToken,
            ResolveHeroProjectileHit,
            ReturnHeroProjectileToPool);
    }

    private int GetCurrentStageToken()
    {
        return currentStageToken;
    }

    private void ResolveHeroProjectileHit(HeroProjectile projectile, EnemyUnit target, float damageSnapshot, int projectileStageToken)
    {
        if (projectileStageToken != currentStageToken || target == null || !target.IsAlive)
        {
            return;
        }

        target.TakeDamage(damageSnapshot);
        Debug.Log($"Hero projectile dealt {damageSnapshot} damage. Enemy HP: {target.currentHp}/{target.maxHp}");
    }

    private static bool IsWithinRange(Transform attacker, Transform target, float range)
    {
        if (attacker == null || target == null)
        {
            return false;
        }

        float safeRange = Mathf.Max(range, 0f);
        Vector3 distanceVector = target.position - attacker.position;
        return distanceVector.sqrMagnitude <= safeRange * safeRange;
    }

    private int CalculateCurrentStageGoldReward()
    {
        if (stageManager == null || stageDifficultyCalculator == null)
        {
            return enemy != null ? Mathf.Max(enemy.goldReward, 10) : 10;
        }

        return stageDifficultyCalculator.CalculateGoldReward(
            stageManager.CurrentArea,
            stageManager.CurrentStage,
            stageManager.MaxStagePerArea);
    }

    private void ResetAttackTimers()
    {
        heroAttackTimer = hero != null ? Mathf.Max(hero.castInterval, 0f) : 0f;
        spiritAttackTimer = spirit != null ? Mathf.Max(spirit.castInterval, 0f) : 0f;
        enemyAttackTimer = enemy != null ? Mathf.Max(enemy.attackInterval, 0f) : 0f;
    }
}
