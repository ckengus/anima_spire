using UnityEngine;

public sealed class SaveRepositoryDebugTester : MonoBehaviour
{
    [SerializeField] private bool runOnStart = false;

    private readonly LocalProgressSaveRepository repository = new LocalProgressSaveRepository();

    private void Start()
    {
        if (runOnStart)
        {
            RunSaveLoadTest();
        }
    }

    [ContextMenu("Run Save Load Test")]
    public void RunSaveLoadTest()
    {
        PlayerProgressData data = CreateDummyData();

        Debug.Log($"Progress save path: {repository.SaveFilePath}");

        bool saveSucceeded = repository.TrySave(data);
        Debug.Log($"Progress save succeeded: {saveSucceeded}");

        bool loadSucceeded = repository.TryLoad(out PlayerProgressData loadedData);
        Debug.Log($"Progress load succeeded: {loadSucceeded}");
        Debug.Log(FormatLoadedData(loadedData));

        if (loadSucceeded)
        {
            Debug.Log($"Progress save JSON:\n{JsonUtility.ToJson(loadedData, true)}");
        }
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSaveFile()
    {
        Debug.Log($"Progress save path: {repository.SaveFilePath}");
        bool deleteSucceeded = repository.TryDelete();
        Debug.Log($"Progress delete succeeded: {deleteSucceeded}");
    }

    private static PlayerProgressData CreateDummyData()
    {
        PlayerProgressData data = PlayerProgressData.CreateDefault();
        data.dataVersion = 1;
        data.lastClearedArea = 1;
        data.lastClearedStage = 7;
        data.gold = 123;
        data.ownedEquipment.Add(new EquipmentSaveData("AMagicBook", "T0", 2));
        data.ownedEquipment.Add(new EquipmentSaveData("BMagicBook", "T0", 1));
        data.equippedMagicBookKey = "AMagicBook:T0";
        return data;
    }

    private static string FormatLoadedData(PlayerProgressData data)
    {
        if (data == null)
        {
            return "Loaded progress data is null.";
        }

        string equipmentText = data.ownedEquipment == null
            ? "null"
            : string.Join(", ", data.ownedEquipment.ConvertAll(item => $"{item.id}:{item.tier} x{item.count}"));

        return $"Loaded progress data: dataVersion={data.dataVersion}, lastClearedArea={data.lastClearedArea}, lastClearedStage={data.lastClearedStage}, gold={data.gold}, ownedEquipment=[{equipmentText}], equippedMagicBookKey={data.equippedMagicBookKey}";
    }
}
