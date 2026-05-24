using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private HeroUnit hero;
    [SerializeField] private SpiritUnit spirit;
    [SerializeField] private EnemyUnit enemy;

    private float heroAttackTimer;
    private float spiritAttackTimer;
    private float enemyAttackTimer;

    private void Start()
    {
        Debug.Log("CombatManager initialized.");
    }

    private void Update()
    {
        if (hero == null || spirit == null || enemy == null)
        {
            return;
        }

        if (hero.IsAlive && enemy.IsAlive)
        {
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
}
