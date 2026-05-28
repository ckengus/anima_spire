public readonly struct EquipmentTierWeightEntry
{
    public EquipmentTierWeightEntry(EquipmentTier tier, int weight)
    {
        this.tier = tier;
        this.weight = weight;
    }

    public readonly EquipmentTier tier;
    public readonly int weight;
}
