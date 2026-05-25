using System;
using System.Collections.Generic;

[Serializable]
public sealed class PlayerProgressData
{
    public int dataVersion;
    public int lastClearedArea;
    public int lastClearedStage;
    public int gold;
    public List<EquipmentSaveData> ownedEquipment;
    public string equippedMagicBookKey;

    public static PlayerProgressData CreateDefault()
    {
        return new PlayerProgressData
        {
            dataVersion = 1,
            lastClearedArea = 1,
            lastClearedStage = 0,
            gold = 0,
            ownedEquipment = new List<EquipmentSaveData>(),
            equippedMagicBookKey = string.Empty
        };
    }
}
