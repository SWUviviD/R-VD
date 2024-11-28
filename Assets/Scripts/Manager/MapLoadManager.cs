using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵 데이터를 로드하고, 로드한 데이터를 기반으로 맵을 생성하는 매니저.
/// </summary>
public class MapLoadManager : MonoSingleton<MapLoadManager>
{
    public void LoadMap(string _mapName)
    {
        var mapData = StageManager.Instance.LoadStage(_mapName);
        
        foreach (var blinkBoard in mapData.BlinkBoardDataList)
        {
            // 인스턴스 생성
            BlinkBoardGimmick instance = CreateGimmick<BlinkBoardGimmick>(blinkBoard.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(blinkBoard);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }
    }

    /// <summary>
    /// 기믹 인스턴스를 생성한다.
    /// </summary>
    private T CreateGimmick<T>(string _address) where T : IGimmickBase
    {
        GameObject prefab = (GameObject)AddressableAssetsManager.Instance.SyncLoadObject(_address, _address);
        T instance = Instantiate(prefab).GetComponent<T>();
        return instance;
    }
}
