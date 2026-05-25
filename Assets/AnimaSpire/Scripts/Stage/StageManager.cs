using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentArea = 1;
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int lastClearedArea = 1;
    [SerializeField] private int lastClearedStage = 0;
    [SerializeField] private int maxStagePerArea = 10;

    public int CurrentArea => currentArea;
    public int CurrentStage => currentStage;
    public int LastClearedArea => lastClearedArea;
    public int LastClearedStage => lastClearedStage;
    public int MaxStagePerArea => maxStagePerArea;

    public event Action<int, int> OnLastClearedStageChanged;

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
        UpdateLastClearedFromCurrentStage();

        currentStage++;

        if (currentStage > maxStagePerArea)
        {
            currentArea++;
            currentStage = 1;
        }

        Debug.Log($"Stage: {GetCurrentStageLogLabel()}");
    }

    public void SetStageForLoad(int area, int stage)
    {
        currentArea = Mathf.Max(area, 1);
        currentStage = Mathf.Clamp(stage, 1, maxStagePerArea);
        Debug.Log($"Stage loaded: {GetCurrentStageLogLabel()}");
    }

    public void SetProgressForLoad(int challengeArea, int challengeStage, int loadedLastClearedArea, int loadedLastClearedStage)
    {
        lastClearedArea = Mathf.Max(loadedLastClearedArea, 1);
        lastClearedStage = Mathf.Clamp(loadedLastClearedStage, 0, maxStagePerArea);
        SetStageForLoad(challengeArea, challengeStage);
    }

    public void RetreatStageOnFailure()
    {
        if (currentStage > 1)
        {
            currentStage--;
        }

        Debug.Log($"Stage: {GetCurrentStageLogLabel()}");
    }

    private void UpdateLastClearedFromCurrentStage()
    {
        int currentIndex = CalculateStageIndex(currentArea, currentStage);
        int lastClearedIndex = lastClearedStage <= 0 ? 0 : CalculateStageIndex(lastClearedArea, lastClearedStage);

        if (currentIndex <= lastClearedIndex)
        {
            return;
        }

        lastClearedArea = currentArea;
        lastClearedStage = currentStage;
        OnLastClearedStageChanged?.Invoke(lastClearedArea, lastClearedStage);
    }

    private int CalculateStageIndex(int area, int stage)
    {
        int safeArea = Mathf.Max(area, 1);
        int safeStage = Mathf.Clamp(stage, 1, maxStagePerArea);
        return (safeArea - 1) * maxStagePerArea + safeStage;
    }
}
