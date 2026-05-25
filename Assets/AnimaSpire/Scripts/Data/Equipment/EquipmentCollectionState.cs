using System;
using System.Collections.Generic;

public sealed class EquipmentCollectionState
{
    private readonly Dictionary<EquipmentStackKey, int> ownedCounts = new Dictionary<EquipmentStackKey, int>();

    public event Action<EquipmentId, EquipmentTier, int> OnCountChanged;

    public int GetOwnedCount(EquipmentId id, EquipmentTier tier)
    {
        EquipmentStackKey key = new EquipmentStackKey(id, tier);
        return ownedCounts.TryGetValue(key, out int count) ? count : 0;
    }

    public int AddOwnedCount(EquipmentId id, EquipmentTier tier, int amount = 1)
    {
        if (amount <= 0)
        {
            return GetOwnedCount(id, tier);
        }

        EquipmentStackKey key = new EquipmentStackKey(id, tier);
        int nextCount = GetOwnedCount(id, tier) + amount;
        ownedCounts[key] = nextCount;
        OnCountChanged?.Invoke(id, tier, nextCount);
        return nextCount;
    }

    public bool HasOwned(EquipmentId id, EquipmentTier tier)
    {
        return GetOwnedCount(id, tier) >= 1;
    }
}
