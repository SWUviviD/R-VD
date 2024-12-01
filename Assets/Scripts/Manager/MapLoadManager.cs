using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

        foreach(var galaxy in mapData.GalaxyGimmickDataList)
        {
            // 인스턴스 생성
            GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(galaxy.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(galaxy);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach(var runandgun in mapData.RunandgunGimmickDataList)
        {
            // 인스턴스 생성
            GameObject instance = CreateGimmickObj(runandgun.Address);
            if(instance.TryGetComponent<RunandgunGimmick>(out var com1))
            {
                com1.GimmickData.Set(runandgun);
                com1.SetGimmick();
            }
            else if(instance.TryGetComponent<RunandgunGimmickHeal>(out var com2))
            {
                com2.GimmickData.Set(runandgun);
                com2.SetGimmick();
            }
        }

        foreach(var chasingGimmick in mapData.ChasingGimmickDataList)
        {
            // 인스턴스 생성
            ChasingGimmick instance = CreateGimmick<ChasingGimmick>(chasingGimmick.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(chasingGimmick);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }



        foreach (var balancePlate in mapData.BalancePlateDataList)
        {
            // 인스턴스 생성
            BalancePlate instance = CreateGimmick<BalancePlate>(balancePlate.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(balancePlate);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var orangeCristal in mapData.CristalGimmickDataList)
        {
            // 인스턴스 생성
            OrangeCristalGimmick instance = CreateGimmick<OrangeCristalGimmick>(orangeCristal.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(orangeCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var blueCristal in mapData.BlueCristalGimmickDataList)
        {
            // 인스턴스 생성
            BlueCristalGimmick instance = CreateGimmick<BlueCristalGimmick>(blueCristal.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(blueCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var greenCristal in mapData.GreenCristalGimmickDataList)
        {
            // 인스턴스 생성
            GreenCristalGimmick instance = CreateGimmick<GreenCristalGimmick>(greenCristal.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(greenCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var Bubble in mapData.BubbleDataList)
        {
            // 인스턴스 생성
            Bubble instance = CreateGimmick<Bubble>(Bubble.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(Bubble);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }


        foreach (var levelObject in mapData.LevelEditObjectList)
        {
            // 인스턴스 생성
            LevelEditObject instance = CreateGimmick<LevelEditObject>(levelObject.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(levelObject);
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

    private GameObject CreateGimmickObj(string _address)
    {
        GameObject prefab = (GameObject)AddressableAssetsManager.Instance.SyncLoadObject(_address, _address);
        GameObject instance = Instantiate(prefab);
        return instance;
    }
}
