using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-9000)]
public sealed class ProgressSaveManager : MonoBehaviour
{
    private const int SupportedDataVersion = 1;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private float saveIntervalSeconds = 60f;
    [SerializeField] private float importantSaveDelaySeconds = 2f;

    private readonly LocalProgressSaveRepository localRepository = new LocalProgressSaveRepository();

    private bool isDirty;
    private bool runtimeEventsBound;
    private bool saveSoonScheduled;
    private bool isSaving;
    private bool isResetting;
    private float nextSaveSoonTime;
    private float nextPeriodicSaveTime;

    public string SaveFilePath => localRepository.SaveFilePath;
    private IProgressSaveRepository Repository => localRepository;

    private void Awake()
    {
        nextPeriodicSaveTime = Time.unscaledTime + Mathf.Max(saveIntervalSeconds, 1f);
    }

    private void Update()
    {
        if (isResetting)
        {
            return;
        }

        float now = Time.unscaledTime;
        bool savedThisFrame = false;

        if (saveSoonScheduled && now >= nextSaveSoonTime)
        {
            if (isResetting)
            {
                return;
            }

            saveSoonScheduled = false;
            SaveIfDirty();
            savedThisFrame = true;
        }

        if (now >= nextPeriodicSaveTime)
        {
            nextPeriodicSaveTime = now + Mathf.Max(saveIntervalSeconds, 1f);

            if (!savedThisFrame && !isResetting)
            {
                SaveIfDirty();
            }
        }
    }

    private void OnDestroy()
    {
        UnbindRuntimeEvents();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || isResetting)
        {
            return;
        }

        if (isDirty || saveSoonScheduled)
        {
            SaveNow();
        }
    }

    private void OnApplicationQuit()
    {
        if (isResetting)
        {
            return;
        }

        SaveIfDirty();
    }

    public bool LoadAndApplyData()
    {
        EnsureReferences();

        if (!Repository.HasSave())
        {
            Debug.Log($"No progress save file found. Starting with default runtime state. Path: {localRepository.SaveFilePath}");
            return false;
        }

        if (!Repository.TryLoad(out PlayerProgressData data))
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
        stageManager?.SetProgressForLoad(challengeStage.area, challengeStage.stage, data.lastClearedArea, data.lastClearedStage);

        List<KeyValuePair<EquipmentStackKey, int>> ownedStacks = BuildOwnedStacksForLoad(data.ownedEquipment);
        equipmentManager?.ReplaceOwnedStacksForLoad(ownedStacks);
        equipmentManager?.SetEquippedMagicBookForLoad(ParseEquippedMagicBookForLoad(data.equippedMagicBookKey));
        equipmentManager?.SetWeaponSlotLevelForLoad(data.weaponSlotLevel);

        combatManager?.RestartCombatForLoadedProgress();

        Debug.Log($"Progress loaded. lastCleared={Mathf.Max(data.lastClearedArea, 1)}-{Mathf.Clamp(data.lastClearedStage, 0, 10)}, challenge={challengeStage.area}-{challengeStage.stage}, gold={Mathf.Max(data.gold, 0)}");
        return true;
    }

    public void BindRuntimeEvents()
    {
        EnsureReferences();

        if (runtimeEventsBound)
        {
            return;
        }

        if (gameManager != null)
        {
            gameManager.OnGoldChanged += HandleGoldChanged;
        }

        if (stageManager != null)
        {
            stageManager.OnLastClearedStageChanged += HandleLastClearedStageChanged;
        }

        if (equipmentManager != null)
        {
            equipmentManager.OnEquipmentSynthesized += HandleEquipmentSynthesized;
            equipmentManager.OnEquippedMagicBookChangedByGameplay += HandleEquippedMagicBookChangedByGameplay;
            equipmentManager.OnWeaponSlotUpgraded += HandleWeaponSlotUpgraded;
        }

        runtimeEventsBound = true;
    }

    public void UnbindRuntimeEvents()
    {
        if (!runtimeEventsBound)
        {
            return;
        }

        if (gameManager != null)
        {
            gameManager.OnGoldChanged -= HandleGoldChanged;
        }

        if (stageManager != null)
        {
            stageManager.OnLastClearedStageChanged -= HandleLastClearedStageChanged;
        }

        if (equipmentManager != null)
        {
            equipmentManager.OnEquipmentSynthesized -= HandleEquipmentSynthesized;
            equipmentManager.OnEquippedMagicBookChangedByGameplay -= HandleEquippedMagicBookChangedByGameplay;
            equipmentManager.OnWeaponSlotUpgraded -= HandleWeaponSlotUpgraded;
        }

        runtimeEventsBound = false;
    }

    public void MarkDirty()
    {
        if (isResetting)
        {
            return;
        }

        isDirty = true;
    }

    public void ScheduleSaveSoon()
    {
        if (isResetting)
        {
            return;
        }

        MarkDirty();

        if (saveSoonScheduled)
        {
            return;
        }

        saveSoonScheduled = true;
        nextSaveSoonTime = Time.unscaledTime + Mathf.Max(importantSaveDelaySeconds, 0f);
    }

    public bool SaveIfDirty()
    {
        if (isResetting)
        {
            return false;
        }

        if (!isDirty)
        {
            return false;
        }

        return SaveNow();
    }

    public bool SaveNow()
    {
        if (isResetting)
        {
            return false;
        }

        if (isSaving)
        {
            return false;
        }

        isSaving = true;

        try
        {
            PlayerProgressData snapshot = BuildSnapshotData();
            bool succeeded = Repository.TrySave(snapshot);

            if (succeeded)
            {
                isDirty = false;
                saveSoonScheduled = false;
                Debug.Log($"Progress saved. Path: {localRepository.SaveFilePath}");
                return true;
            }

            isDirty = true;
            Debug.LogWarning("Progress save failed. Dirty state remains active.");
            return false;
        }
        catch (Exception exception)
        {
            isDirty = true;
            Debug.LogWarning($"Progress save failed: {exception.Message}");
            return false;
        }
        finally
        {
            isSaving = false;
        }
    }

    public void ResetProgressForDebug()
    {
        isResetting = true;
        isDirty = false;
        saveSoonScheduled = false;

        if (isSaving)
        {
            isResetting = false;
            Debug.LogWarning("Debug reset progress was skipped because a progress save is already running.");
            return;
        }

        try
        {
            if (!Repository.TryDelete())
            {
                isResetting = false;
                Debug.LogWarning($"Debug reset progress failed. Scene reload skipped. Path: {localRepository.SaveFilePath}");
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            Debug.Log($"Debug reset progress completed. Reloading scene: {activeScene.name}");
            SceneManager.LoadScene(activeScene.buildIndex);
        }
        catch (Exception exception)
        {
            isResetting = false;
            Debug.LogWarning($"Debug reset progress failed: {exception.Message}");
        }
    }

    private PlayerProgressData BuildSnapshotData()
    {
        EnsureReferences();

        PlayerProgressData data = PlayerProgressData.CreateDefault();
        data.dataVersion = SupportedDataVersion;
        data.gold = gameManager != null ? gameManager.GetGold() : 0;

        if (stageManager != null)
        {
            data.lastClearedArea = stageManager.LastClearedArea;
            data.lastClearedStage = stageManager.LastClearedStage;
        }

        if (equipmentManager != null)
        {
            data.weaponSlotLevel = equipmentManager.WeaponSlotLevel;

            List<KeyValuePair<EquipmentStackKey, int>> stacks = equipmentManager.GetOwnedStacksSnapshot();
            for (int i = 0; i < stacks.Count; i++)
            {
                KeyValuePair<EquipmentStackKey, int> stack = stacks[i];
                if (stack.Value <= 0)
                {
                    continue;
                }

                data.ownedEquipment.Add(new EquipmentSaveData(stack.Key.id.ToString(), stack.Key.tier.ToString(), stack.Value));
            }

            if (equipmentManager.TryGetEquippedMagicBook(out EquipmentStackKey equippedKey))
            {
                data.equippedMagicBookKey = $"{equippedKey.id}:{equippedKey.tier}";
            }
        }

        return data;
    }

    private void HandleGoldChanged(int currentGold)
    {
        MarkDirty();
    }

    private void HandleLastClearedStageChanged(int area, int stage)
    {
        MarkDirty();
    }

    private void HandleEquipmentSynthesized(EquipmentId id, EquipmentTier tier)
    {
        ScheduleSaveSoon();
    }

    private void HandleEquippedMagicBookChangedByGameplay(EquipmentStackKey? key)
    {
        ScheduleSaveSoon();
    }

    private void HandleWeaponSlotUpgraded(int slotLevel)
    {
        ScheduleSaveSoon();
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
