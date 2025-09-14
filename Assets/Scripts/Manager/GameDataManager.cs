using MemoryPack;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum StageID
{
    Stage1 = 1,
    //Stage2,
    //Stage3,
    MAX
}

[MemoryPackable]
[Serializable]
public partial class GameData
{
    [MemoryPackInclude] public StageID StageID { get; set; }
    [MemoryPackInclude] public int PlayerHealth { get; set; }
    [MemoryPackInclude] public Vector3 PlayerPosition { get; set; }
    [MemoryPackInclude] public Vector3 PlayerRotation { get; set; }
    [MemoryPackInclude] public Vector3 camRotation { get; set; }
}

public class GameDataManager : MonoBehaviour
{
    private static readonly string SavedDataPath = 
        Application.persistentDataPath + "/save";
    private static readonly string FileName = "GameData";

    private static string BuildSavePath
        => Path.Combine(Application.persistentDataPath, "Save");
    private static string BuildSaveFilePath
        => Path.Combine(BuildSavePath, "GameData.bytes");

    public GameData GameData { get; private set; }

    public bool LoadGameData()
    {
#if UNITY_EDITOR
        List<GameData> list;
        if (!SerializeManager.Instance.LoadDataFile(out list, FileName)
            || list == null
            || list.Count == 0
            || list[0] == null)
        {
            Debug.LogWarning("[SaveSystem] LoadGameData: No valid data found. Creating new GameData.");
            GameData = new GameData();
            return false; // 새로 생성했음을 알림
        }
        else
        {
            GameData = list[0];
            return true;
        }

#else
        if (Directory.Exists(BuildSavePath) == false)
        {
            Directory.CreateDirectory(BuildSavePath);
        }

        if (File.Exists(BuildSaveFilePath) == false)
        {
            GameData = new GameData();
            return false;
        }

        byte[] bytes = File.ReadAllBytes(BuildSaveFilePath);
        var list = MemoryPackSerializer.Deserialize<List<GameData>>(bytes);

        if(list == null || list.Count <= 0)
        {
            GameData = new GameData();
            return false;
        }

        GameData = list[0];
        return true;

#endif
    }

    public void SaveGameData(GameData newData = null)
    {
        if(newData != null)
        {
            GameData = newData;
        }

#if UNITY_EDITOR
        List<GameData> list = new List<GameData>(1) { GameData };
        var byteArray = MemoryPackSerializer.Serialize(list);

        SerializeManager.Instance.SaveDataFile(FileName, byteArray);
#else
        if (Directory.Exists(BuildSavePath) == false)
        {
            Directory.CreateDirectory(BuildSavePath);
        }

        List<GameData> list = new List<GameData>(1) { GameData };
        var byteArray = MemoryPackSerializer.Serialize(list);

        File.WriteAllBytes(BuildSaveFilePath, byteArray);
        return;
#endif
    }

    public void SaveGameData(StageID stageID, int playerHealth,
        Vector3 playerPosition, Vector3 playerRotation,
        Vector3 camRotation)
    {
        GameData.StageID = stageID;
        GameData.PlayerHealth = playerHealth;
        GameData.PlayerPosition = playerPosition;
        GameData.PlayerRotation = playerRotation;
        GameData.camRotation = camRotation;

        SaveGameData(GameData);
    }


    public void ResetGameData()
    {
        GameData.StageID = StageID.Stage1;
        GameData.PlayerHealth = 10;
        GameData.PlayerPosition = Vector3.zero;
        GameData.PlayerRotation = Vector3.zero;
        GameData.camRotation = Vector3.right * 180f;

        SaveGameData(GameData);
    }

    public void DeleteGameData()
    {
        List<GameData> list = new List<GameData>();

        var byteArray = MemoryPackSerializer.Serialize(list);

        SerializeManager.Instance.SaveDataFile(FileName, byteArray);
    }
}
