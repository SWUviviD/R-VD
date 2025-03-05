#if UNITY_EDITOR
using LevelEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestScript : MonoBehaviour
{
#if UNITY_EDITOR
    // 실험을 위한 임시 추가
    [SerializeField] PlacementSystem placementSystem;
#endif

    [SerializeField] private string mapName;

    private void Start()
    {
        MapLoadManager.Instance.LoadMap(mapName);
    }
}
