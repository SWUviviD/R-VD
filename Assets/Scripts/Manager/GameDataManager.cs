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

    public GameData GameData { get; private set; }

    public bool LoadGameData()
    {
        FileInfo fileInfo = new FileInfo(SavedDataPath);
        if (fileInfo.Exists)
        {
            List<GameData> list;
            if (SerializeManager.Instance.LoadDataFile<GameData>(out list, FileName) == false)
            {
                GameData = new GameData();
                return false;
            }

            GameData = list[0];
        }
        return true;
    }

    public void SaveGameData(GameData newData = null)
    {
        if(newData != null)
        {
            GameData = newData;
        }

        List<GameData> list = new List<GameData>(1);
        list.Add(GameData);

        var byteArray = MemoryPackSerializer.Serialize(list);

        SerializeManager.Instance.SaveDataFile(FileName, byteArray);
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

    public void RemoveGameData()
    {
        File.Delete(SavedDataPath + FileName);
    }

    public void ResetGameData()
    {
        GameData.StageID = StageID.Stage1;
        GameData.PlayerHealth = 10;
        GameData.PlayerPosition = Vector3.zero;
        GameData.PlayerRotation = Vector3.zero;
        GameData.camRotation = Vector3.zero;

        SaveGameData(GameData);
    }
}
