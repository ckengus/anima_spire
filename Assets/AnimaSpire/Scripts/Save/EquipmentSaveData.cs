using System;

[Serializable]
public sealed class EquipmentSaveData
{
    public string id;
    public string tier;
    public int count;

    public EquipmentSaveData()
    {
        id = string.Empty;
        tier = string.Empty;
        count = 0;
    }

    public EquipmentSaveData(string id, string tier, int count)
    {
        this.id = id;
        this.tier = tier;
        this.count = count;
    }
}
