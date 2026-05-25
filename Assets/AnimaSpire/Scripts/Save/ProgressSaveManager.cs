using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-9000)]
public sealed class ProgressSaveManager : MonoBehaviour
{
    private const int SupportedDataVersion = 1;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private CombatManager combatManager;

    private readonly LocalProgressSaveRepository repository = new LocalProgressSaveRepository();

    public string SaveFilePath => repository.SaveFilePath;

    public bool LoadAndApplyData()
    {
        EnsureReferences();

        if (!repository.HasSave())
        {
            Debug.Log($"No progress save file found. Starting with default runtime state. Path: {repository.SaveFilePath}");
            return false;
        }

        if (!repository.TryLoad(out PlayerProgressData data))
        {
            Debug.LogWarning("Progress save load failed. Starting with default runtime state.");
            return false;
        }

        return TryApplyProgressData(data);
    }

    private bool TryApplyProgressData(PlayerProgressData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Progress data is null. Starting with default runtime state.");
            return false;
        }

        if (data.dataVersion != SupportedDataVersion)
        {
            Debug.LogWarning($"Unsupported progress data version: {data.dataVersion}. Expected: {SupportedDataVersion}. Starting with default runtime state.");
            return false;
        }

        ChallengeStage challengeStage = CalculateChallengeStageFromLastCleared(data.lastClearedArea, data.lastClearedStage);

        gameManager?.SetGoldForLoad(data.gold);
        stageManager?.SetStageForLoad(challengeStage.area, challengeStage.stage);

        List<KeyValuePair<EquipmentStackKey, int>> ownedStacks = BuildOwnedStacksForLoad(data.ownedEquipment);
        equipmentManager?.ReplaceOwnedStacksForLoad(ownedStacks);
        equipmentManager?.SetEquippedMagicBookForLoad(ParseEquippedMagicBookForLoad(data.equippedMagicBookKey));

        combatManager?.RestartCombatForLoadedProgress();

        Debug.Log($"Progress loaded. lastCleared={Mathf.Max(data.lastClearedArea, 1)}-{Mathf.Clamp(data.lastClearedStage, 0, 10)}, challenge={challengeStage.area}-{challengeStage.stage}, gold={Mathf.Max(data.gold, 0)}");
        return true;
    }

    private List<KeyValuePair<EquipmentStackKey, int>> BuildOwnedStacksForLoad(List<EquipmentSaveData> savedEquipment)
    {
        Dictionary<EquipmentStackKey, int> mergedStacks = new Dictionary<EquipmentStackKey, int>();

        if (savedEquipment == null)
        {
            return new List<KeyValuePair<EquipmentStackKey, int>>();
        }

        foreach (EquipmentSaveData savedStack in savedEquipment)
        {
            if (savedStack == null)
            {
                Debug.LogWarning("Invalid equipment save entry: null.");
                continue;
            }

            if (savedStack.count <= 0)
            {
                Debug.LogWarning($"Invalid equipment count ignored: {savedStack.id}:{savedStack.tier} x{savedStack.count}");
                continue;
            }

            if (!TryParseEquipmentKey(savedStack.id, savedStack.tier, out EquipmentStackKey key))
            {
                continue;
            }

            if (!EquipmentCatalog.TryGetDefinition(key.id, key.tier, out EquipmentDefinition _))
            {
                Debug.LogWarning($"Unknown equipment definition ignored: {savedStack.id}:{savedStack.tier}");
                continue;
            }

            mergedStacks[key] = mergedStacks.TryGetValue(key, out int existingCount)
                ? existingCount + savedStack.count
                : savedStack.count;
        }

        return new List<KeyValuePair<EquipmentStackKey, int>>(mergedStacks);
    }

    private EquipmentStackKey? ParseEquippedMagicBookForLoad(string rawKey)
    {
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            return null;
        }

        string[] parts = rawKey.Split(':');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"Invalid equipped MagicBook key ignored: {rawKey}");
            return null;
        }

        if (!TryParseEquipmentKey(parts[0], parts[1], out EquipmentStackKey key))
        {
            Debug.LogWarning($"Invalid equipped MagicBook key ignored: {rawKey}");
            return null;
        }

        if (key.id != EquipmentId.AMagicBook && key.id != EquipmentId.BMagicBook)
        {
            Debug.LogWarning($"Non-MagicBook equipped key ignored: {rawKey}");
            return null;
        }

        if (equipmentManager == null || !equipmentManager.HasOwned(key.id, key.tier))
        {
            Debug.LogWarning($"Equipped MagicBook is not owned and was ignored: {rawKey}");
            return null;
        }

        return key;
    }

    private bool TryParseEquipmentKey(string idText, string tierText, out EquipmentStackKey key)
    {
        if (string.IsNullOrWhiteSpace(idText))
        {
            Debug.LogWarning("Invalid equipment id ignored: empty");
            key = default;
            return false;
        }

        if (string.IsNullOrWhiteSpace(tierText))
        {
            Debug.LogWarning($"Invalid equipment tier ignored for {idText}: empty");
            key = default;
            return false;
        }

        if (!Enum.TryParse(idText, out EquipmentId id))
        {
            Debug.LogWarning($"Invalid equipment id ignored: {idText}");
            key = default;
            return false;
        }

        if (!Enum.TryParse(tierText, out EquipmentTier tier))
        {
            Debug.LogWarning($"Invalid equipment tier ignored: {tierText}");
            key = default;
            return false;
        }

        key = new EquipmentStackKey(id, tier);
        return true;
    }

    private ChallengeStage CalculateChallengeStageFromLastCleared(int lastClearedArea, int lastClearedStage)
    {
        int safeArea = Mathf.Max(lastClearedArea, 1);
        int maxStage = stageManager != null ? Mathf.Max(stageManager.MaxStagePerArea, 1) : 10;
        int safeLastClearedStage = lastClearedStage;

        if (safeLastClearedStage < 0)
        {
            safeLastClearedStage = 0;
        }

        if (safeLastClearedStage > maxStage)
        {
            Debug.LogWarning($"lastClearedStage exceeds max stage per area and was clamped: {safeLastClearedStage}");
            safeLastClearedStage = maxStage;
        }

        if (safeLastClearedStage == 0)
        {
            return new ChallengeStage(1, 1);
        }

        if (safeLastClearedStage < maxStage)
        {
            return new ChallengeStage(safeArea, safeLastClearedStage + 1);
        }

        return new ChallengeStage(safeArea + 1, 1);
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

        if (equipmentManager == null)
        {
            equipmentManager = EquipmentManager.EnsureInstance();
        }

        if (combatManager == null)
        {
            combatManager = FindAnyObjectByType<CombatManager>();
        }
    }

    private readonly struct ChallengeStage
    {
        public ChallengeStage(int area, int stage)
        {
            this.area = area;
            this.stage = stage;
        }

        public readonly int area;
        public readonly int stage;
    }
}
