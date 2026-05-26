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
    [SerializeField] private int baseGoldReward = 10;
    [SerializeField] private int goldRewardGrowthPerStage = 5;

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
        int difficultyIndex = CalculateEnemyDifficultyIndex(area, stage, stageCountPerArea);
        float enemyMaxHp = baseEnemyHp + (difficultyIndex - 1) * hpGrowthPerStage;
        float enemyAttackPower = baseEnemyAttack + (difficultyIndex - 1) * attackGrowthPerStage;

        return new EnemyStageStats(enemyMaxHp, enemyAttackPower);
    }

    public int CalculateEnemyDifficultyIndex(int area, int stage)
    {
        return CalculateEnemyDifficultyIndex(area, stage, maxStagePerArea);
    }

    public int CalculateEnemyDifficultyIndex(int area, int stage, int stageCountPerArea)
    {
        int safeArea = Mathf.Max(area, 1);
        int safeMaxStagePerArea = Mathf.Max(stageCountPerArea, 1);
        int safeStage = Mathf.Clamp(stage, 1, safeMaxStagePerArea);
        int areaDifficultyStep = Mathf.Max(safeMaxStagePerArea - 2, 1);

        return ((safeArea - 1) * areaDifficultyStep) + safeStage;
    }

    public int CalculateGoldReward(int area, int stage)
    {
        return CalculateGoldReward(area, stage, maxStagePerArea);
    }

    public int CalculateGoldReward(int area, int stage, int stageCountPerArea)
    {
        int globalStageIndex = CalculateStageIndex(area, stage, stageCountPerArea);
        int reward = baseGoldReward + (globalStageIndex - 1) * goldRewardGrowthPerStage;
        return Mathf.Max(reward, baseGoldReward);
    }
}
