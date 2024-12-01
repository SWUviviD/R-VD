using LevelEditor;
using LocalData;
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

        foreach (var levelObject in mapData.LevelEditObjectList)
        {
            // 인스턴스 생성
            LevelEditObject instance = CreateGimmick<LevelEditObject>(levelObject.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(levelObject);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var galaxy in mapData.GalaxyGimmickDataList)
        {
            // 인스턴스 생성
            GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(galaxy.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(galaxy);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var runandgun in mapData.RunandgunGimmickDataList)
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

        foreach (var chasingGimmick in mapData.ChasingGimmickDataList)
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

        if (mapData.PlayerPositionSettor != null)
        {
            PlayerPositionSettor instance = CreateGimmick<PlayerPositionSettor>(mapData.PlayerPositionSettor.Address);
            instance.GimmickData.Set(mapData.PlayerPositionSettor);
            instance.SetGimmick();
        }
    }

    public void LoadMapInEditor(string _mapName)
    {
        var placementSystem = FindObjectOfType<PlacementSystem>();
        var mapData = StageManager.Instance.LoadStage(_mapName);

        foreach (var blinkBoard in mapData.BlinkBoardDataList)
        {
            placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, blinkBoard);
            //// 인스턴스 생성
            //BlinkBoardGimmick instance = CreateGimmick<BlinkBoardGimmick>(blinkBoard.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(blinkBoard);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var levelObject in mapData.LevelEditObjectList)
        {
            placementSystem.CreateGimmick(levelObject.Address, levelObject.Position, levelObject.Rotation, levelObject.Scale, levelObject);
            //// 인스턴스 생성
            //LevelEditObject instance = CreateGimmick<LevelEditObject>(levelObject.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(levelObject);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var galaxy in mapData.GalaxyGimmickDataList)
        {
            placementSystem.CreateGimmick(galaxy.Address, galaxy.Position, galaxy.Rotation, galaxy.Scale, galaxy);
            //// 인스턴스 생성
            //GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(galaxy.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(galaxy);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var runandgun in mapData.RunandgunGimmickDataList)
        {
            placementSystem.CreateGimmick(runandgun.Address, runandgun.Position, runandgun.Rotation, runandgun.Scale, runandgun);
            //// 인스턴스 생성
            //GameObject instance = CreateGimmickObj(runandgun.Address);
            //if(instance.TryGetComponent<RunandgunGimmick>(out var com1))
            //{
            //    com1.GimmickData.Set(runandgun);
            //    com1.SetGimmick();
            //}
            //else if(instance.TryGetComponent<RunandgunGimmickHeal>(out var com2))
            //{
            //    com2.GimmickData.Set(runandgun);
            //    com2.SetGimmick();
            //}
        }

        foreach (var chasingGimmick in mapData.ChasingGimmickDataList)
        {
            placementSystem.CreateGimmick(chasingGimmick.Address, chasingGimmick.Position, chasingGimmick.Rotation, chasingGimmick.Scale, chasingGimmick);
            //// 인스턴스 생성
            //ChasingGimmick instance = CreateGimmick<ChasingGimmick>(chasingGimmick.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(chasingGimmick);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }



        foreach (var balancePlate in mapData.BalancePlateDataList)
        {
            placementSystem.CreateGimmick(balancePlate.Address, balancePlate.Position, balancePlate.Rotation, balancePlate.Scale, balancePlate);
            //// 인스턴스 생성
            //BalancePlate instance = CreateGimmick<BalancePlate>(balancePlate.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(balancePlate);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var orangeCristal in mapData.CristalGimmickDataList)
        {
            placementSystem.CreateGimmick(orangeCristal.Address, orangeCristal.Position, orangeCristal.Rotation, orangeCristal.Scale, orangeCristal);
            //// 인스턴스 생성
            //OrangeCristalGimmick instance = CreateGimmick<OrangeCristalGimmick>(orangeCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(orangeCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var blueCristal in mapData.BlueCristalGimmickDataList)
        {
            placementSystem.CreateGimmick(blueCristal.Address, blueCristal.Position, blueCristal.Rotation, blueCristal.Scale, blueCristal);
            //// 인스턴스 생성
            //BlueCristalGimmick instance = CreateGimmick<BlueCristalGimmick>(blueCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(blueCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var greenCristal in mapData.GreenCristalGimmickDataList)
        {
            placementSystem.CreateGimmick(greenCristal.Address, greenCristal.Position, greenCristal.Rotation, greenCristal.Scale, greenCristal);
            //// 인스턴스 생성
            //GreenCristalGimmick instance = CreateGimmick<GreenCristalGimmick>(greenCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(greenCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var Bubble in mapData.BubbleDataList)
        {
            placementSystem.CreateGimmick(Bubble.Address, Bubble.Position, Bubble.Rotation, Bubble.Scale, Bubble);
            //// 인스턴스 생성
            //Bubble instance = CreateGimmick<Bubble>(Bubble.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(Bubble);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        if (mapData.PlayerPositionSettor != null)
        {
            placementSystem.CreateGimmick(mapData.PlayerPositionSettor.Address,
                                          mapData.PlayerPositionSettor.Position,
                                          mapData.PlayerPositionSettor.Rotation,
                                          mapData.PlayerPositionSettor.Scale,
                                          mapData.PlayerPositionSettor);
            //PlayerPositionSettor instance = CreateGimmick<PlayerPositionSettor>(mapData.PlayerPositionSettor.Address);
            //instance.GimmickData.Set(mapData.PlayerPositionSettor);
            //instance.SetGimmick();
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
