#if UNITY_EDITOR
using LevelEditor;
#endif
using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵 데이터를 로드하고, 로드한 데이터를 기반으로 맵을 생성하는 매니저.
/// </summary>
public class MapLoadManager : MonoSingleton<MapLoadManager>
{
    public LDMapData MapData { get; private set; }
    public GameObject LoadMap(string _mapName)
    {
        MapData = StageManager.Instance.LoadStage(_mapName);

        foreach (var blinkBoard in MapData.BlinkBoardDataList)
        {
            // 인스턴스 생성
            BlinkBoardGimmick instance = CreateGimmick<BlinkBoardGimmick>(blinkBoard.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(blinkBoard);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var levelObject in MapData.LevelEditObjectList)
        {
            // 인스턴스 생성
            LevelEditObject instance = CreateGimmick<LevelEditObject>(levelObject.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(levelObject);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var galaxy in MapData.GalaxyGimmickDataList)
        {
            // 인스턴스 생성
            GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(galaxy.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(galaxy);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var runandgun in MapData.RunandgunGimmickDataList)
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

        foreach (var chasingGimmick in MapData.ChasingGimmickDataList)
        {
            // 인스턴스 생성
            ChasingGimmick instance = CreateGimmick<ChasingGimmick>(chasingGimmick.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(chasingGimmick);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }



        foreach (var balancePlate in MapData.BalancePlateDataList)
        {
            // 인스턴스 생성
            BalancePlate instance = CreateGimmick<BalancePlate>(balancePlate.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(balancePlate);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var orangeCristal in MapData.CristalGimmickDataList)
        {
            // 인스턴스 생성
            OrangeCristalGimmick instance = CreateGimmick<OrangeCristalGimmick>(orangeCristal.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(orangeCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var blueCristal in MapData.BlueCristalGimmickDataList)
        {
            // 인스턴스 생성
            BlueCristalGimmick instance = CreateGimmick<BlueCristalGimmick>(blueCristal.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(blueCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var greenCristal in MapData.GreenCristalGimmickDataList)
        {
            // 인스턴스 생성
            GreenCristalGimmick instance = CreateGimmick<GreenCristalGimmick>(greenCristal.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(greenCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var Bubble in MapData.BubbleDataList)
        {
            // 인스턴스 생성
            Bubble instance = CreateGimmick<Bubble>(Bubble.Address);
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(Bubble);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        GameObject player = null;
        if (MapData.PlayerPositionSettor != null)
        {
            PlayerPositionSettor instance = CreateGimmick<PlayerPositionSettor>(MapData.PlayerPositionSettor.Address);
            instance.GimmickData.Set(MapData.PlayerPositionSettor);
            instance.SetGimmick();

            player = instance.Player;
        }

        if(MapData.StageClearPoint != null)
        {
            StageClearPoint instance = CreateGimmick<StageClearPoint>(MapData.StageClearPoint.Address);
            instance.GimmickData.Set(MapData.StageClearPoint);
            instance.SetGimmick();
        }

        foreach(var checkpoint in MapData.CheckpointList)
        {
            CheckpointGimmick instance = CreateGimmick<CheckpointGimmick>(checkpoint.Address);
            instance.GimmickData.Set(checkpoint);
            instance.SetGimmick();
        }

        return player;
    }

#if UNITY_EDITOR
    public void LoadMapInEditor(string _mapName)
    {
        var placementSystem = FindObjectOfType<PlacementSystem>();
        MapData = StageManager.Instance.LoadStage(_mapName);

        foreach (var blinkBoard in MapData.BlinkBoardDataList)
        {
            placementSystem.CreateGimmick(blinkBoard.Address, blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, blinkBoard);
            //// 인스턴스 생성
            //BlinkBoardGimmick instance = CreateGimmick<BlinkBoardGimmick>(blinkBoard.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(blinkBoard);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var levelObject in MapData.LevelEditObjectList)
        {
            placementSystem.CreateGimmick(levelObject.Address, levelObject.Position, levelObject.Rotation, levelObject.Scale, levelObject);
            //// 인스턴스 생성
            //LevelEditObject instance = CreateGimmick<LevelEditObject>(levelObject.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(levelObject);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var galaxy in MapData.GalaxyGimmickDataList)
        {
            placementSystem.CreateGimmick(galaxy.Address, galaxy.Position, galaxy.Rotation, galaxy.Scale, galaxy);
            //// 인스턴스 생성
            //GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(galaxy.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(galaxy);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var runandgun in MapData.RunandgunGimmickDataList)
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

        foreach (var chasingGimmick in MapData.ChasingGimmickDataList)
        {
            placementSystem.CreateGimmick(chasingGimmick.Address, chasingGimmick.Position, chasingGimmick.Rotation, chasingGimmick.Scale, chasingGimmick);
            //// 인스턴스 생성
            //ChasingGimmick instance = CreateGimmick<ChasingGimmick>(chasingGimmick.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(chasingGimmick);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }



        foreach (var balancePlate in MapData.BalancePlateDataList)
        {
            placementSystem.CreateGimmick(balancePlate.Address, balancePlate.Position, balancePlate.Rotation, balancePlate.Scale, balancePlate);
            //// 인스턴스 생성
            //BalancePlate instance = CreateGimmick<BalancePlate>(balancePlate.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(balancePlate);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var orangeCristal in MapData.CristalGimmickDataList)
        {
            placementSystem.CreateGimmick(orangeCristal.Address, orangeCristal.Position, orangeCristal.Rotation, orangeCristal.Scale, orangeCristal);
            //// 인스턴스 생성
            //OrangeCristalGimmick instance = CreateGimmick<OrangeCristalGimmick>(orangeCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(orangeCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var blueCristal in MapData.BlueCristalGimmickDataList)
        {
            placementSystem.CreateGimmick(blueCristal.Address, blueCristal.Position, blueCristal.Rotation, blueCristal.Scale, blueCristal);
            //// 인스턴스 생성
            //BlueCristalGimmick instance = CreateGimmick<BlueCristalGimmick>(blueCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(blueCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var greenCristal in MapData.GreenCristalGimmickDataList)
        {
            placementSystem.CreateGimmick(greenCristal.Address, greenCristal.Position, greenCristal.Rotation, greenCristal.Scale, greenCristal);
            //// 인스턴스 생성
            //GreenCristalGimmick instance = CreateGimmick<GreenCristalGimmick>(greenCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(greenCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var Bubble in MapData.BubbleDataList)
        {
            placementSystem.CreateGimmick(Bubble.Address, Bubble.Position, Bubble.Rotation, Bubble.Scale, Bubble);
            //// 인스턴스 생성
            //Bubble instance = CreateGimmick<Bubble>(Bubble.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(Bubble);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        if (MapData.PlayerPositionSettor != null)
        {
            placementSystem.CreateGimmick(MapData.PlayerPositionSettor.Address,
                                          MapData.PlayerPositionSettor.Position,
                                          MapData.PlayerPositionSettor.Rotation,
                                          MapData.PlayerPositionSettor.Scale,
                                          MapData.PlayerPositionSettor);
            //PlayerPositionSettor instance = CreateGimmick<PlayerPositionSettor>(mapData.PlayerPositionSettor.Address);
            //instance.GimmickData.Set(mapData.PlayerPositionSettor);
            //instance.SetGimmick();
        }

        if(MapData.StageClearPoint != null)
        {
            LDStageClearPointData pointData = MapData.StageClearPoint;
            placementSystem.CreateGimmick(pointData.Address, pointData.Position, pointData.Rotation, pointData.Scale, pointData);
        }

        foreach (var checkpoint in MapData.CheckpointList)
        {
            placementSystem.CreateGimmick(checkpoint.Address, checkpoint.Position, checkpoint.Rotation, checkpoint.Scale, checkpoint);
        }
        
        CameraPathInsertSystem.Instance.LoadPath(MapData.CameraPathList);
    }
#endif

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
