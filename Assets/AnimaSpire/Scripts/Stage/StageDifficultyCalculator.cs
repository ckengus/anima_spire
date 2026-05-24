using UnityEngine;

public struct EnemyStageStats
{
    public EnemyStageStats(float maxHp, float attackPower)
    {
        MaxHp = maxHp;
        AttackPower = attackPower;
    }

    public float MaxHp { get; }
    public float AttackPower { get; }
}

public class StageDifficultyCalculator : MonoBehaviour
{
    [SerializeField] private int maxStagePerArea = 10;
    [SerializeField] private float baseEnemyHp = 50f;
    [SerializeField] private float hpGrowthPerStage = 10f;
    [SerializeField] private float baseEnemyAttack = 5f;
    [SerializeField] private float attackGrowthPerStage = 1f;

    public int CalculateStageIndex(int area, int stage)
    {
        return CalculateStageIndex(area, stage, maxStagePerArea);
    }

    public int CalculateStageIndex(int area, int stage, int stageCountPerArea)
    {
        int safeArea = Mathf.Max(area, 1);
        int safeStage = Mathf.Max(stage, 1);
        int safeMaxStagePerArea = Mathf.Max(stageCountPerArea, 1);

        return (safeArea - 1) * safeMaxStagePerArea + safeStage;
    }

    public EnemyStageStats CalculateEnemyStats(int area, int stage)
    {
        return CalculateEnemyStats(area, stage, maxStagePerArea);
    }

    public EnemyStageStats CalculateEnemyStats(int area, int stage, int stageCountPerArea)
    {
        int stageIndex = CalculateStageIndex(area, stage, stageCountPerArea);
        float enemyMaxHp = baseEnemyHp + (stageIndex - 1) * hpGrowthPerStage;
        float enemyAttackPower = baseEnemyAttack + (stageIndex - 1) * attackGrowthPerStage;

        return new EnemyStageStats(enemyMaxHp, enemyAttackPower);
    }
}
