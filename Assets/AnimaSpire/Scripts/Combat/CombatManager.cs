using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HeroUnit hero;
    [SerializeField] private SpiritUnit spirit;
    [SerializeField] private EnemyUnit enemy;

    private float heroAttackTimer;
    private float spiritAttackTimer;
    private float enemyAttackTimer;
    private bool isResolvingEnemyDefeat;

    private void Start()
    {
        Debug.Log("CombatManager initialized.");
    }

    private void Update()
    {
        if (gameManager == null || hero == null || spirit == null || enemy == null)
        {
            return;
        }

        if (isResolvingEnemyDefeat || !hero.IsAlive || !enemy.IsAlive)
        {
            if (!isResolvingEnemyDefeat && !enemy.IsAlive)
            {
                isResolvingEnemyDefeat = true;
                StartCoroutine(RewardGoldAndResetCombat());
            }

            return;
        }

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
        isResolvingEnemyDefeat = true;
        gameManager.AddGold(enemy.goldReward);

        yield return new WaitForSeconds(1f);

        hero.ResetHp();
        enemy.ResetHp();
        ResetAttackTimers();
        isResolvingEnemyDefeat = false;
    }

    private void ResetAttackTimers()
    {
        heroAttackTimer = 0f;
        spiritAttackTimer = 0f;
        enemyAttackTimer = 0f;
    }
}
