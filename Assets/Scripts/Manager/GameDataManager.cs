using MemoryPack;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum StageID
{
    Stage1 = 1,
    Stage2 = 2,
    Stage3 = 3,
    MAX
}

[MemoryPackable]
[Serializable]
public partial class GameData
{
    [MemoryPackInclude] public StageID StageID { get; set; }
    [MemoryPackInclude] public int CheckPointID { get; set; }
    [MemoryPackInclude] public int PlayerHealth { get; set; }
    [MemoryPackInclude] public Vector3 PlayerPosition { get; set; }
    [MemoryPackInclude] public Vector3 PlayerRotation { get; set; }
    [MemoryPackInclude] public Vector3 camRotation { get; set; }
    [MemoryPackInclude] public bool IsSkill1_StarHuntUnlocked {  get; set; }
    [MemoryPackInclude] public bool IsSkill2_StarFusionUnlocked {  get; set; }
    [MemoryPackInclude] public bool IsSkill3_WaterVaseUnlocked {  get; set; }
    [MemoryPackInclude] public int TryTimes { get; set; }
    [MemoryPackInclude] public int LastTryCheckPointID { get; set; }
    [MemoryPackInclude] public uint Flags { get; set; }
}

public class GameDataManager
{
    private static readonly string SavedDataPath = 
        Application.persistentDataPath + "/save";
    private static readonly string FileName = "GameData";

    private static string BuildSavePath
        => Path.Combine(Application.persistentDataPath, "Save");
    private static string BuildSaveFilePath
        => Path.Combine(BuildSavePath, "GameData.bytes");

    private static GameData gameData;
    public static GameData GameData { get => gameData; private set => gameData = value; }

    public GameData GetGameData()
    {
        return GameData;
    }

    private bool isGameOvered = false;

#if UNITY_EDITOR
    [MenuItem("Data/DeleteData")]
    public static void DeleteData()
    {
        SerializeManager.Instance.DeleteDataFile(FileName);
    }
#endif

    public bool LoadGameData()
    {
#if UNITY_EDITOR
        List<GameData> list;
        if (isGameOvered == true
            || !SerializeManager.Instance.LoadDataFile(out list, FileName)
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

            if (GameManager.Instance.GetFlag(10) == true)
            {
                GameData = new GameData();
                return false;
            }

            Debug.LogError($"{gameData.CheckPointID} {gameData.PlayerPosition} {gameData.PlayerRotation}");
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
        
        if (GameManager.Instance.GetFlag(10) == true)
        {
            GameData = new GameData();
            return false;
        }

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

    public void SaveGameData(StageID stageID, int checkPointID, int playerHealth,
        Vector3 playerPosition, Vector3 playerRotation, Vector3 camRotation, 
        bool isSkill1Unlocked, bool isSkill2Unlocked, bool isSkill3Unlocked, 
        int tryTimes, int lastTryCheckPoint, uint Flag)
    {
        GameData.StageID = stageID;
        GameData.CheckPointID = checkPointID;
        GameData.PlayerHealth = playerHealth;
        GameData.PlayerPosition = playerPosition;
        GameData.PlayerRotation = playerRotation;
        GameData.camRotation = camRotation;
        GameData.IsSkill1_StarHuntUnlocked = isSkill1Unlocked;
        GameData.IsSkill2_StarFusionUnlocked = isSkill2Unlocked;
        GameData.IsSkill3_WaterVaseUnlocked = isSkill3Unlocked;
        GameData.TryTimes = tryTimes;
        GameData.LastTryCheckPointID = lastTryCheckPoint;
        GameData.Flags = Flag;

        SaveGameData(GameData);
    }


    public void ResetGameData()
    {
        GameData.StageID = StageID.Stage1;
        GameData.CheckPointID = 100;
        GameData.PlayerHealth = 10;
        GameData.PlayerPosition = Vector3.zero;
        GameData.PlayerRotation = Vector3.zero;
        GameData.camRotation = Vector3.right * 180f;
        GameData.IsSkill1_StarHuntUnlocked = false;
        GameData.IsSkill2_StarFusionUnlocked = false;
        GameData.IsSkill3_WaterVaseUnlocked = false;
        GameData.TryTimes = 0;
        GameData.Flags = 0;

        SaveGameData(GameData);
    }

    public void DeleteGameData()
    {
        GameManager.Instance.SetFlag(10, true);
        
        GameData.StageID = StageID.Stage1;
        isGameOvered = true;

#if UNITY_EDITOR
        SerializeManager.Instance.DeleteDataFile(FileName);
#else
        try
        {
            // BuildSavePath는 파일 이름까지 포함된 전체 경로여야 합니다.
            if (File.Exists(BuildSaveFilePath))
            {
                File.Delete(BuildSaveFilePath);
                
                Debug.Log($"File Deleted Successfully: {BuildSaveFilePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Delete File Fail: " + e.Message);
        }
#endif
    }
}
