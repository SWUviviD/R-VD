using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestScript : MonoBehaviour
{
    private void Start()
    {
        var mapData = StageManager.Instance.LoadStage("asdf");
        
        foreach (var blinkBoard in mapData.BlinkBoardDataList)
        {
            GameObject prefab = (GameObject)AddressableAssetsManager.Instance.SyncLoadObject(blinkBoard.Address, blinkBoard.Address);
            var instance = Instantiate(prefab).GetComponent<BlinkBoardGimmick>();
            instance.GimmickData.Set(blinkBoard);
        }
    }
}
