using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentArea = 1;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int maxStagePerArea = 10;

    public int CurrentArea => currentArea;
    public int CurrentStage => currentStage;
    public int MaxStagePerArea => maxStagePerArea;

    private void Start()
    {
        Debug.Log($"Stage: {GetCurrentStageLogLabel()}");
    }

    public string GetCurrentStageLabel()
    {
        return $"{currentArea}-{currentStage}";
    }

    public bool IsBossStage()
    {
        return currentStage == maxStagePerArea;
    }

    public string GetCurrentStageLogLabel()
    {
        if (IsBossStage())
        {
            return $"{GetCurrentStageLabel()} (Boss)";
        }

        return GetCurrentStageLabel();
    }

    public void AdvanceStage()
    {
        currentStage++;

        if (currentStage > maxStagePerArea)
        {
            currentArea++;
            currentStage = 1;
        }

        Debug.Log($"Stage: {GetCurrentStageLogLabel()}");
    }

    public void RetreatStageOnFailure()
    {
        if (currentStage > 1)
        {
            currentStage--;
        }

        Debug.Log($"Stage: {GetCurrentStageLogLabel()}");
    }
}
