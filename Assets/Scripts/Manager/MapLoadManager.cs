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
            LevelEditObject instance = CreateGimmick<LevelEditObject>(ChangeAddress(levelObject.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(levelObject);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var galaxy in MapData.GalaxyGimmickDataList)
        {
            // 인스턴스 생성
            GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(ChangeAddress(galaxy.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(galaxy);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var runandgun in MapData.RunandgunGimmickDataList)
        {
            // 인스턴스 생성
            GameObject instance = CreateGimmickObj(ChangeAddress(runandgun.Address));
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
            ChasingGimmick instance = CreateGimmick<ChasingGimmick>(ChangeAddress(chasingGimmick.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(chasingGimmick);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }



        foreach (var balancePlate in MapData.BalancePlateDataList)
        {
            // 인스턴스 생성
            BalancePlate instance = CreateGimmick<BalancePlate>(ChangeAddress(balancePlate.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(balancePlate);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var orangeCristal in MapData.CristalGimmickDataList)
        {
            // 인스턴스 생성
            OrangeCristalGimmick instance = CreateGimmick<OrangeCristalGimmick>(ChangeAddress(orangeCristal.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(orangeCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var blueCristal in MapData.BlueCristalGimmickDataList)
        {
            // 인스턴스 생성
            BlueCristalGimmick instance = CreateGimmick<BlueCristalGimmick>(ChangeAddress(blueCristal.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(blueCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var greenCristal in MapData.GreenCristalGimmickDataList)
        {
            // 인스턴스 생성
            GreenCristalGimmick instance = CreateGimmick<GreenCristalGimmick>(ChangeAddress(greenCristal.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(greenCristal);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        foreach (var Bubble in MapData.BubbleDataList)
        {
            // 인스턴스 생성
            Bubble instance = CreateGimmick<Bubble>(ChangeAddress(Bubble.Address));
            // 인스턴스에 데이터 세팅
            instance.GimmickData.Set(Bubble);
            // 기믹이 동작할 수 있도록 생성
            instance.SetGimmick();
        }

        GameObject player = null;
        if (MapData.PlayerPositionSettor != null)
        {
            PlayerPositionSettor instance = CreateGimmick<PlayerPositionSettor>(ChangeAddress(MapData.PlayerPositionSettor.Address));
            instance.GimmickData.Set(MapData.PlayerPositionSettor);
            instance.SetGimmick();

            player = instance.Player;
        }

        if(MapData.StageClearPoint != null)
        {
            StageClearPoint instance = CreateGimmick<StageClearPoint>(ChangeAddress(MapData.StageClearPoint.Address));
            instance.GimmickData.Set(MapData.StageClearPoint);
            instance.SetGimmick();
        }

        foreach(var checkpoint in MapData.CheckpointList)
        {
            CheckpointGimmick instance = CreateGimmick<CheckpointGimmick>(ChangeAddress(checkpoint.Address));
            instance.GimmickData.Set(checkpoint);
            instance.SetGimmick();
        }

        return player;
    }

    private string ChangeAddress(string address)
    {
        if (address[0] != 'A') return address;
        string header = address.Substring(0, 7);
        string trailer = address.Substring(7);
        return trailer.Split('.')[0];
    }

#if UNITY_EDITOR
    public void LoadMapInEditor(string _mapName)
    {
        var placementSystem = FindObjectOfType<PlacementSystem>();
        MapData = StageManager.Instance.LoadStage(_mapName);

        foreach (var blinkBoard in MapData.BlinkBoardDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(blinkBoard.Address), blinkBoard.Position, blinkBoard.Rotation, blinkBoard.Scale, blinkBoard);
            //// 인스턴스 생성
            //BlinkBoardGimmick instance = CreateGimmick<BlinkBoardGimmick>(blinkBoard.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(blinkBoard);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var levelObject in MapData.LevelEditObjectList)
        {
            placementSystem.CreateGimmick(ChangeAddress(levelObject.Address), levelObject.Position, levelObject.Rotation, levelObject.Scale, levelObject);
            //// 인스턴스 생성
            //LevelEditObject instance = CreateGimmick<LevelEditObject>(levelObject.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(levelObject);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var galaxy in MapData.GalaxyGimmickDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(galaxy.Address), galaxy.Position, galaxy.Rotation, galaxy.Scale, galaxy);
            //// 인스턴스 생성
            //GalaxyGimmick instance = CreateGimmick<GalaxyGimmick>(galaxy.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(galaxy);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var runandgun in MapData.RunandgunGimmickDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(runandgun.Address), runandgun.Position, runandgun.Rotation, runandgun.Scale, runandgun);
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
            placementSystem.CreateGimmick(ChangeAddress(chasingGimmick.Address), chasingGimmick.Position, chasingGimmick.Rotation, chasingGimmick.Scale, chasingGimmick);
            //// 인스턴스 생성
            //ChasingGimmick instance = CreateGimmick<ChasingGimmick>(chasingGimmick.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(chasingGimmick);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }



        foreach (var balancePlate in MapData.BalancePlateDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(balancePlate.Address), balancePlate.Position, balancePlate.Rotation, balancePlate.Scale, balancePlate);
            //// 인스턴스 생성
            //BalancePlate instance = CreateGimmick<BalancePlate>(balancePlate.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(balancePlate);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var orangeCristal in MapData.CristalGimmickDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(orangeCristal.Address), orangeCristal.Position, orangeCristal.Rotation, orangeCristal.Scale, orangeCristal);
            //// 인스턴스 생성
            //OrangeCristalGimmick instance = CreateGimmick<OrangeCristalGimmick>(orangeCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(orangeCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var blueCristal in MapData.BlueCristalGimmickDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(blueCristal.Address), blueCristal.Position, blueCristal.Rotation, blueCristal.Scale, blueCristal);
            //// 인스턴스 생성
            //BlueCristalGimmick instance = CreateGimmick<BlueCristalGimmick>(blueCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(blueCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var greenCristal in MapData.GreenCristalGimmickDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(greenCristal.Address), greenCristal.Position, greenCristal.Rotation, greenCristal.Scale, greenCristal);
            //// 인스턴스 생성
            //GreenCristalGimmick instance = CreateGimmick<GreenCristalGimmick>(greenCristal.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(greenCristal);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        foreach (var Bubble in MapData.BubbleDataList)
        {
            placementSystem.CreateGimmick(ChangeAddress(Bubble.Address), Bubble.Position, Bubble.Rotation, Bubble.Scale, Bubble);
            //// 인스턴스 생성
            //Bubble instance = CreateGimmick<Bubble>(Bubble.Address);
            //// 인스턴스에 데이터 세팅
            //instance.GimmickData.Set(Bubble);
            //// 기믹이 동작할 수 있도록 생성
            //instance.SetGimmick();
        }

        if (MapData.PlayerPositionSettor != null)
        {
            placementSystem.CreateGimmick(ChangeAddress(MapData.PlayerPositionSettor.Address),
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
            placementSystem.CreateGimmick(ChangeAddress(pointData.Address), pointData.Position, pointData.Rotation, pointData.Scale, pointData);
        }

        foreach (var checkpoint in MapData.CheckpointList)
        {
            placementSystem.CreateGimmick(ChangeAddress(checkpoint.Address), checkpoint.Position, checkpoint.Rotation, checkpoint.Scale, checkpoint);
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
