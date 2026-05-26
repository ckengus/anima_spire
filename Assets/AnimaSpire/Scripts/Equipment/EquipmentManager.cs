using System;
using UnityEngine;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    private const int MagicBookSummonCost = 10;
    private const int WeaponSlotUpgradeCost = 10;
    private const int WeaponSlotAttackBonusPerLevel = 1;

    [SerializeField] private GameManager gameManager;

    private readonly EquipmentCollectionState collectionState = new EquipmentCollectionState();
    private readonly EquipmentLoadoutState loadoutState = new EquipmentLoadoutState();
    private int weaponSlotLevel;

    public int MagicBookCost => MagicBookSummonCost;
    public int WeaponSlotLevel => weaponSlotLevel;
    public static EquipmentManager Instance { get; private set; }
    public EquipmentCollectionState CollectionState => collectionState;
    public EquipmentLoadoutState LoadoutState => loadoutState;

    public event Action<EquipmentId, EquipmentTier> OnMagicBookSummoned;
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
        summonedDefinition = null;

        if (gameManager == null)
        {
            message = "Equipment system is not ready.";
            return false;
        }

        EquipmentId summonedId = UnityEngine.Random.value < 0.5f ? EquipmentId.AMagicBook : EquipmentId.BMagicBook;
        EquipmentTier tier = EquipmentTier.T0;

        if (!EquipmentCatalog.TryGetDefinition(summonedId, tier, out summonedDefinition))
        {
            message = "MagicBook data is missing.";
            return false;
        }

        if (!gameManager.TrySpendGold(MagicBookSummonCost))
        {
            summonedDefinition = null;
            message = "Not enough Gold.";
            return false;
        }

        collectionState.AddOwnedCount(summonedId, tier, 1);
        OnMagicBookSummoned?.Invoke(summonedId, tier);
        message = $"Summoned {summonedDefinition.displayName} {tier}";
        return true;
    }

    public int GetWeaponSlotUpgradeCost(int currentLevel)
    {
        return WeaponSlotUpgradeCost;
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
