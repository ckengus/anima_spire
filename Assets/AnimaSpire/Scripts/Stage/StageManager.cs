using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentArea = 1;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int maxStagePerArea = 10;

    private void Start()
    {
        Debug.Log($"Stage: {GetCurrentStageLabel()}");
    }

    public string GetCurrentStageLabel()
    {
        return $"{currentArea}-{currentStage}";
    }

    public void AdvanceStage()
    {
        currentStage++;

        if (currentStage > maxStagePerArea)
        {
            currentArea++;
            currentStage = 1;
        }

        Debug.Log($"Stage: {GetCurrentStageLabel()}");
    }
}
