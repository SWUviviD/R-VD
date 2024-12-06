using System;
using System.Collections;
using System.Collections.Generic;
using LocalData;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private GameObject player;
    
    private void Start()
    {
        // 맵을 로드한다.
        MapLoadManager.Instance.LoadMap("map");
        player = GameObject.FindGameObjectWithTag("Player");
        LDMapData mapData = MapLoadManager.Instance.MapData;
        var cameraController = CameraController.Instance;
        // 카메라가 플레이어를 따라가도록 한다.
        cameraController.SetPlayer(player.transform);
        // 카메라 경로를 넣고 동작시킨다.
        cameraController.Set(mapData.CameraPathList.ConvertAll(_ => _.ToCameraPathPoint()));
        cameraController.Play();
    }
}
