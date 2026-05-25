using System;
using System.Collections.Generic;

[Serializable]
public sealed class PlayerProgressData
{
    public int dataVersion;
    public int currentArea;
    public int currentStage;
    public int gold;
    public List<EquipmentSaveData> ownedEquipment;
    public string equippedMagicBookKey;

    public static PlayerProgressData CreateDefault()
    {
        return new PlayerProgressData
        {
            dataVersion = 1,
            currentArea = 1,
            currentStage = 1,
            gold = 0,
            ownedEquipment = new List<EquipmentSaveData>(),
            equippedMagicBookKey = string.Empty
        };
    }
}
