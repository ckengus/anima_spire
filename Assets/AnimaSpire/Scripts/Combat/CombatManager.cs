using System.Collections;
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

    private float heroAttackTimer;
    private float spiritAttackTimer;
    private float enemyAttackTimer;
    private bool isResolvingCombat;

    private void Start()
    {
        EnsureReferences();

        ResetRuntimePositionsForCombat();
        ApplyCurrentStageEnemyStats();
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

        UpdateEnemyMovement();

        heroAttackTimer += Time.deltaTime;
        spiritAttackTimer += Time.deltaTime;

        if (heroAttackTimer >= hero.castInterval)
        {
            heroAttackTimer = 0f;
            hero.DealDamage(enemy);
        }

        if (spiritAttackTimer >= spirit.castInterval)
        {
            spiritAttackTimer = 0f;
            spirit.DealDamage(enemy);
        }

        if (enemy.IsAlive && hero.IsAlive)
        {
            enemyAttackTimer += Time.deltaTime;

            if (enemyAttackTimer >= enemy.attackInterval)
            {
                enemyAttackTimer = 0f;
                enemy.DealDamage(hero);
            }
        }
    }

    private IEnumerator RewardGoldAndResetCombat()
    {
        isResolvingCombat = true;
        gameManager.AddGold(CalculateCurrentStageGoldReward());
        stageManager?.AdvanceStage();

        yield return new WaitForSeconds(1f);

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

        string retreatStageLabel = stageManager != null ? stageManager.GetCurrentStageLabel() : "Unknown";
        Debug.Log($"Retreat to Stage: {retreatStageLabel}");

        yield return new WaitForSeconds(1f);

        ResetRuntimePositionsForCombat();
        hero.ResetHp();
        ApplyCurrentStageEnemyStats();
        ResetAttackTimers();
        isResolvingCombat = false;
    }

    public void RestartCombatForLoadedProgress()
    {
        EnsureReferences();
        StopAllCoroutines();
        isResolvingCombat = false;
        ResetRuntimePositionsForCombat();
        hero?.ResetHp();
        ApplyCurrentStageEnemyStats();
        ResetAttackTimers();
        Debug.Log("Combat restarted for loaded progress.");
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

    private void UpdateEnemyMovement()
    {
        if (hero == null || enemy == null || !hero.IsAlive || !enemy.IsAlive)
        {
            return;
        }

        Transform heroTransform = hero.transform;
        Transform enemyTransform = enemy.transform;
        Vector3 heroPosition = heroTransform.position;
        Vector3 enemyPosition = enemyTransform.position;
        float stopDistance = Mathf.Max(enemyStopDistance, 0f);
        float currentDistance = Vector3.Distance(enemyPosition, heroPosition);

        if (currentDistance <= stopDistance || Mathf.Approximately(currentDistance, 0f))
        {
            return;
        }

        Vector3 directionFromHeroToEnemy = (enemyPosition - heroPosition).normalized;
        Vector3 stopPosition = heroPosition + directionFromHeroToEnemy * stopDistance;
        float maxDistanceDelta = Mathf.Max(enemyMoveSpeed, 0f) * Time.deltaTime;
        enemyTransform.position = Vector3.MoveTowards(enemyPosition, stopPosition, maxDistanceDelta);
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
        heroAttackTimer = 0f;
        spiritAttackTimer = 0f;
        enemyAttackTimer = 0f;
    }
}
