public readonly struct EquipmentSummonEntry
{
    public EquipmentSummonEntry(EquipmentId id, int weight)
    {
        this.id = id;
        this.weight = weight;
    }

    public readonly EquipmentId id;
    public readonly int weight;
}
