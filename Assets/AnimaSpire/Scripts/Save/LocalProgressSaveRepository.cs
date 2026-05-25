using System;
using System.IO;
using UnityEngine;

public sealed class LocalProgressSaveRepository : IProgressSaveRepository
{
    public const string SaveFileName = "player_progress.json";

    public string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public bool HasSave()
    {
        try
        {
            return File.Exists(SaveFilePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to check progress save file: {exception.Message}");
            return false;
        }
    }

    public bool TryLoad(out PlayerProgressData data)
    {
        data = PlayerProgressData.CreateDefault();

        try
        {
            string path = SaveFilePath;
            if (!File.Exists(path))
            {
                return false;
            }

            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning("Progress save file is empty.");
                return false;
            }

            PlayerProgressData loadedData = JsonUtility.FromJson<PlayerProgressData>(json);
            if (loadedData == null)
            {
                Debug.LogWarning("Progress save file could not be parsed.");
                return false;
            }

            if (loadedData.ownedEquipment == null)
            {
                loadedData.ownedEquipment = new System.Collections.Generic.List<EquipmentSaveData>();
            }

            if (loadedData.equippedMagicBookKey == null)
            {
                loadedData.equippedMagicBookKey = string.Empty;
            }

            data = loadedData;
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to load progress save file: {exception.Message}");
            data = PlayerProgressData.CreateDefault();
            return false;
        }
    }

    public bool TrySave(PlayerProgressData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Cannot save null progress data.");
            return false;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, json);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to save progress data: {exception.Message}");
            return false;
        }
    }

    public bool TryDelete()
    {
        try
        {
            string path = SaveFilePath;
            if (!File.Exists(path))
            {
                return true;
            }

            File.Delete(path);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to delete progress save file: {exception.Message}");
            return false;
        }
    }
}
