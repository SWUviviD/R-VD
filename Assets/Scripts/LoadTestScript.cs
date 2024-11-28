using LevelEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestScript : MonoBehaviour
{
    // 실험을 위한 임시 추가
    [SerializeField] PlacementSystem placementSystem;

    [SerializeField] private string mapName;

    private void Start()
    {
        MapLoadManager.Instance.LoadMap(mapName);
    }
}
