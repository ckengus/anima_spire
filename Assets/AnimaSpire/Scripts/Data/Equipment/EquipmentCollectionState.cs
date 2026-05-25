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

    public void ReplaceOwnedStacksForLoad(IEnumerable<KeyValuePair<EquipmentStackKey, int>> stacks)
    {
        ownedCounts.Clear();

        if (stacks == null)
        {
            return;
        }

        foreach (KeyValuePair<EquipmentStackKey, int> stack in stacks)
        {
            if (stack.Value <= 0)
            {
                continue;
            }

            ownedCounts[stack.Key] = stack.Value;
        }
    }

    public bool HasOwned(EquipmentId id, EquipmentTier tier)
    {
        return GetOwnedCount(id, tier) >= 1;
    }

    public List<KeyValuePair<EquipmentStackKey, int>> GetOwnedStacksSnapshot()
    {
        return new List<KeyValuePair<EquipmentStackKey, int>>(ownedCounts);
    }
}
