using System;
using UnityEngine;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    private const int EquipmentSynthesisCost = 10;
    private const int WeaponSlotUpgradeCost = 10;
    private const int WeaponSlotAttackBonusPerLevel = 1;

    [SerializeField] private GameManager gameManager;

    private readonly EquipmentCollectionState collectionState = new EquipmentCollectionState();
    private readonly EquipmentLoadoutState loadoutState = new EquipmentLoadoutState();
    private int weaponSlotLevel;

    public int MagicBookCost => EquipmentSynthesisCost;
    public int WeaponSlotLevel => weaponSlotLevel;
    public static EquipmentManager Instance { get; private set; }
    public EquipmentCollectionState CollectionState => collectionState;
    public EquipmentLoadoutState LoadoutState => loadoutState;

#pragma warning disable 0067
    public event Action<EquipmentId, EquipmentTier> OnMagicBookSummoned;
#pragma warning restore 0067
    public event Action<EquipmentId, EquipmentTier> OnEquipmentSynthesized;
    public event Action<EquipmentStackKey?> OnEquippedMagicBookChangedByGameplay;
    public event Action<int> OnWeaponSlotUpgraded;

    public static EquipmentManager EnsureInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        EquipmentManager found = FindAnyObjectByType<EquipmentManager>();
        if (found != null)
        {
            Instance = found;
            return Instance;
        }

        GameObject managerObject = new GameObject("EquipmentManager");
        return managerObject.AddComponent<EquipmentManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate EquipmentManager found. Using the first instance.");
            return;
        }

        Instance = this;

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    public bool TrySummonMagicBook(out EquipmentDefinition summonedDefinition, out string message)
    {
        return TrySynthesizeEquipment(out summonedDefinition, out message);
    }

    public bool TrySynthesizeEquipment(out EquipmentDefinition synthesizedDefinition, out string message)
    {
        return TrySynthesizeEquipment(null, out synthesizedDefinition, out message);
    }

    public bool TrySynthesizeEquipment(IReadOnlyCollection<EquipmentCategory> categories, out EquipmentDefinition synthesizedDefinition, out string message)
    {
        synthesizedDefinition = null;

        if (gameManager == null)
        {
            message = "\uC7A5\uBE44 \uC2DC\uC2A4\uD15C\uC774 \uC900\uBE44\uB418\uC9C0 \uC54A\uC558\uC2B5\uB2C8\uB2E4.";
            return false;
        }

        if (gameManager.GetGold() < EquipmentSynthesisCost)
        {
            message = "Gold\uAC00 \uBD80\uC871\uD569\uB2C8\uB2E4.";
            return false;
        }

        List<EquipmentSummonEntry> equipmentCandidates = BuildEquipmentCandidates(categories);
        if (equipmentCandidates.Count == 0)
        {
            message = "\uC5F0\uC131 \uAC00\uB2A5\uD55C \uC7A5\uBE44\uAC00 \uC5C6\uC2B5\uB2C8\uB2E4.";
            return false;
        }

        int equipmentTotalWeight = CalculateTotalWeight(equipmentCandidates);
        if (equipmentTotalWeight <= 0 || !TryRollEquipmentId(equipmentCandidates, equipmentTotalWeight, out EquipmentId selectedId))
        {
            message = "\uC5F0\uC131 \uAC00\uB2A5\uD55C \uC7A5\uBE44\uAC00 \uC5C6\uC2B5\uB2C8\uB2E4.";
            return false;
        }

        List<TierCandidate> tierCandidates = BuildTierCandidates(selectedId);
        if (tierCandidates.Count == 0)
        {
            message = "\uC5F0\uC131 \uAC00\uB2A5\uD55C \uD2F0\uC5B4\uAC00 \uC5C6\uC2B5\uB2C8\uB2E4.";
            return false;
        }

        int tierTotalWeight = CalculateTotalWeight(tierCandidates);
        if (tierTotalWeight <= 0 || !TryRollTier(tierCandidates, tierTotalWeight, out EquipmentTier selectedTier))
        {
            message = "\uC5F0\uC131 \uAC00\uB2A5\uD55C \uD2F0\uC5B4\uAC00 \uC5C6\uC2B5\uB2C8\uB2E4.";
            return false;
        }

        if (!EquipmentCatalog.TryGetDefinition(selectedId, selectedTier, out EquipmentDefinition finalDefinition))
        {
            message = "\uC7A5\uBE44 \uB370\uC774\uD130\uAC00 \uC5C6\uC2B5\uB2C8\uB2E4.";
            return false;
        }

        if (!gameManager.TrySpendGold(EquipmentSynthesisCost))
        {
            message = "Gold\uAC00 \uBD80\uC871\uD569\uB2C8\uB2E4.";
            return false;
        }

        collectionState.AddOwnedCount(selectedId, selectedTier, 1);
        OnEquipmentSynthesized?.Invoke(selectedId, selectedTier);
        synthesizedDefinition = finalDefinition;
        message = $"\uC5F0\uC131 \uC131\uACF5: {finalDefinition.displayName} {selectedTier}";
        return true;
    }

    private List<EquipmentSummonEntry> BuildEquipmentCandidates(IReadOnlyCollection<EquipmentCategory> categories)
    {
        IReadOnlyList<EquipmentSummonEntry> entries = EquipmentCatalog.GetSummonableEquipmentEntries(categories);
        List<EquipmentSummonEntry> candidates = new List<EquipmentSummonEntry>();
        for (int i = 0; i < entries.Count; i++)
        {
            EquipmentSummonEntry entry = entries[i];
            if (entry.weight > 0)
            {
                candidates.Add(entry);
            }
        }

        return candidates;
    }

    private List<TierCandidate> BuildTierCandidates(EquipmentId selectedId)
    {
        IReadOnlyList<EquipmentTierWeightEntry> entries = EquipmentCatalog.GetSynthesisTierWeightEntries();
        List<TierCandidate> candidates = new List<TierCandidate>();
        for (int i = 0; i < entries.Count; i++)
        {
            EquipmentTierWeightEntry entry = entries[i];
            if (entry.weight <= 0)
            {
                continue;
            }

            if (EquipmentCatalog.TryGetDefinition(selectedId, entry.tier, out EquipmentDefinition _))
            {
                candidates.Add(new TierCandidate(entry.tier, entry.weight));
            }
        }

        return candidates;
    }

    private int CalculateTotalWeight(List<EquipmentSummonEntry> candidates)
    {
        int totalWeight = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            totalWeight += candidates[i].weight;
        }

        return totalWeight;
    }

    private int CalculateTotalWeight(List<TierCandidate> candidates)
    {
        int totalWeight = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            totalWeight += candidates[i].weight;
        }

        return totalWeight;
    }

    private bool TryRollEquipmentId(List<EquipmentSummonEntry> candidates, int totalWeight, out EquipmentId selectedId)
    {
        int roll = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < candidates.Count; i++)
        {
            EquipmentSummonEntry candidate = candidates[i];
            roll -= candidate.weight;
            if (roll < 0)
            {
                selectedId = candidate.id;
                return true;
            }
        }

        selectedId = default;
        return false;
    }

    private bool TryRollTier(List<TierCandidate> candidates, int totalWeight, out EquipmentTier selectedTier)
    {
        int roll = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < candidates.Count; i++)
        {
            TierCandidate candidate = candidates[i];
            roll -= candidate.weight;
            if (roll < 0)
            {
                selectedTier = candidate.tier;
                return true;
            }
        }

        selectedTier = default;
        return false;
    }

    private readonly struct TierCandidate
    {
        public TierCandidate(EquipmentTier tier, int weight)
        {
            this.tier = tier;
            this.weight = weight;
        }

        public readonly EquipmentTier tier;
        public readonly int weight;
    }

    public int GetWeaponSlotUpgradeCost(int currentLevel)
    {
        int safeLevel = Mathf.Max(0, currentLevel);
        return WeaponSlotUpgradeCost * (safeLevel + 1);
    }

    public int GetWeaponSlotAttackBonus(int slotLevel)
    {
        return Mathf.Max(0, slotLevel) * WeaponSlotAttackBonusPerLevel;
    }

    public bool TryUpgradeWeaponSlot(out string message)
    {
        if (gameManager == null)
        {
            message = "Equipment system is not ready.";
            return false;
        }

        int cost = GetWeaponSlotUpgradeCost(weaponSlotLevel);
        if (!gameManager.TrySpendGold(cost))
        {
            message = "Not enough Gold.";
            return false;
        }

        weaponSlotLevel++;
        OnWeaponSlotUpgraded?.Invoke(weaponSlotLevel);
        message = "Slot upgraded.";
        return true;
    }

    public int GetOwnedCount(EquipmentId id, EquipmentTier tier)
    {
        return collectionState.GetOwnedCount(id, tier);
    }

    public bool HasOwned(EquipmentId id, EquipmentTier tier)
    {
        return collectionState.HasOwned(id, tier);
    }

    public bool TryEquipMagicBook(EquipmentId id, out string message)
    {
        EquipmentTier tier = EquipmentTier.T0;

        if (id != EquipmentId.AMagicBook && id != EquipmentId.BMagicBook)
        {
            message = "Only MagicBook equipment can be equipped now.";
            return false;
        }

        if (!EquipmentCatalog.TryGetDefinition(id, tier, out EquipmentDefinition definition))
        {
            message = "MagicBook data is missing.";
            return false;
        }

        if (!collectionState.HasOwned(id, tier))
        {
            message = $"You do not own {definition.displayName} {tier}.";
            return false;
        }

        EquipmentStackKey nextKey = new EquipmentStackKey(id, tier);
        bool changed = !loadoutState.TryGetEquippedMagicBook(out EquipmentStackKey currentKey) || currentKey != nextKey;
        loadoutState.EquipMagicBook(nextKey);
        if (changed)
        {
            OnEquippedMagicBookChangedByGameplay?.Invoke(nextKey);
        }

        message = $"Equipped {definition.displayName} {tier}.";
        return true;
    }

    public bool HasEquippedMagicBook()
    {
        return loadoutState.HasEquippedMagicBook();
    }

    public bool TryGetEquippedMagicBook(out EquipmentStackKey key)
    {
        return loadoutState.TryGetEquippedMagicBook(out key);
    }

    public void ReplaceOwnedStacksForLoad(IEnumerable<KeyValuePair<EquipmentStackKey, int>> stacks)
    {
        collectionState.ReplaceOwnedStacksForLoad(stacks);
    }

    public void SetEquippedMagicBookForLoad(EquipmentStackKey? key)
    {
        if (key.HasValue && !collectionState.HasOwned(key.Value.id, key.Value.tier))
        {
            Debug.LogWarning($"Cannot equip unloaded MagicBook for load: {key.Value.id}:{key.Value.tier}");
            loadoutState.SetEquippedMagicBookForLoad(null);
            return;
        }

        loadoutState.SetEquippedMagicBookForLoad(key);
    }

    public void SetWeaponSlotLevelForLoad(int level)
    {
        weaponSlotLevel = Mathf.Max(level, 0);
    }

    public List<KeyValuePair<EquipmentStackKey, int>> GetOwnedStacksSnapshot()
    {
        return collectionState.GetOwnedStacksSnapshot();
    }

    public int GetEquippedMagicBookBonusAttackPower()
    {
        if (!loadoutState.TryGetEquippedMagicBook(out EquipmentStackKey key))
        {
            return 0;
        }

        if (!EquipmentCatalog.TryGetDefinition(key.id, key.tier, out EquipmentDefinition definition))
        {
            return 0;
        }

        return definition.bonusAttackPower + GetWeaponSlotAttackBonus(weaponSlotLevel);
    }
}
