using System;
using System.Collections;
using System.Collections.Generic;
using LocalData;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject Player { get; private set; }
    
    private void Awake()
    {
        // 맵을 로드한다.
        Player = MapLoadManager.Instance.LoadMap("map");
        LDMapData mapData = MapLoadManager.Instance.MapData;

        if(Player != null)
        {
            var cameraController = CameraController.Instance;
            // 카메라가 플레이어를 따라가도록 한다.
            cameraController.SetPlayer(Player.transform);
            // 카메라 경로를 넣고 동작시킨다.
            cameraController.Set(mapData.CameraPathList.ConvertAll(_ => _.ToCameraPathPoint()));
            cameraController.Play();
        }
    }
}
