public sealed class EquipmentDefinition
{
    public EquipmentDefinition(
        EquipmentId id,
        EquipmentType type,
        EquipmentCategory category,
        EquipmentTier tier,
        string displayName,
        string description,
        int bonusAttackPower,
        int bonusSkillDamage)
    {
        this.id = id;
        this.type = type;
        this.category = category;
        this.tier = tier;
        this.displayName = displayName;
        this.description = description;
        this.bonusAttackPower = bonusAttackPower;
        this.bonusSkillDamage = bonusSkillDamage;
    }

    public EquipmentId id { get; }
    public EquipmentType type { get; }
    public EquipmentCategory category { get; }
    public EquipmentTier tier { get; }
    public string displayName { get; }
    public string description { get; }
    public int bonusAttackPower { get; }
    public int bonusSkillDamage { get; }
}
