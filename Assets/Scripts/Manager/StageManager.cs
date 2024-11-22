using System.Collections;
using System.Collections.Generic;
using LocalData;
using MemoryPack;
using UnityEngine;

/// <summary>
/// 스테이지를 저장하고, 불러오는 매니저
/// </summary>
public class StageManager : MonoSingleton<StageManager>
{
    private List<LDMapData> mapDatas;

    private LDMapData currentMapData;

    protected override void Init()
    {
        base.Init();
        mapDatas = new List<LDMapData>();
    }

    /// <summary>
    /// 스테이지를 저장한다.
    /// </summary>
    public void SaveStage(string _fileName, List<GimmickDataBase> _gimmickDataBases, List<CameraPathPoint> _cameraPath)
    {
        mapDatas.Clear();
        mapDatas.Add(new LDMapData());
        
        foreach (var gimmickData in _gimmickDataBases)
        {
            gimmickData.SaveGimmickData(mapDatas[0]);
        }

        mapDatas[0].CameraPathList = _cameraPath;
        
        byte[] bytes = SerializeManager.Instance.Serialize(mapDatas);
        SerializeManager.Instance.SaveDataFile(_fileName, bytes);
    }

    /// <summary>
    /// 스테이지를 불러온다.
    /// </summary>
    public LDMapData LoadStage(string _fileName)
    {
        List<LDMapData> mapDatas;
        SerializeManager.Instance.LoadDataFile(out mapDatas, _fileName);
        currentMapData = mapDatas[0];
        return currentMapData;
    }

    /// <summary>
    /// 스테이지 관련해 들고있는 데이터를 모두 비운다.
    /// </summary>
    public void ClearStage()
    {
        mapDatas.Clear();
        currentMapData = null;
    }
}
