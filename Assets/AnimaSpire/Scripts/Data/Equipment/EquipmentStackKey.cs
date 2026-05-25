using System;

public readonly struct EquipmentStackKey : IEquatable<EquipmentStackKey>
{
    public EquipmentStackKey(EquipmentId id, EquipmentTier tier)
    {
        this.id = id;
        this.tier = tier;
    }

    public EquipmentId id { get; }
    public EquipmentTier tier { get; }

    public bool Equals(EquipmentStackKey other)
    {
        return id == other.id && tier == other.tier;
    }

    public override bool Equals(object obj)
    {
        return obj is EquipmentStackKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id, tier);
    }

    public static bool operator ==(EquipmentStackKey left, EquipmentStackKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EquipmentStackKey left, EquipmentStackKey right)
    {
        return !left.Equals(right);
    }
}
