using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCristalGimmickData : ColoredCristalData
{
    [GimmickData("공 크기")]
    [field: SerializeField]
    public float SphereSize { get; set; } = 10f;
    
    [GimmickData("공이 특정 목적지까지 도달하는 시간")]
    [field: SerializeField]
    public float SphereMoveTime { get; set; } = 16f;
    
    [GimmickData("공 회전 속도")]
    [field: SerializeField]
    public float SphereRotateSpeed { get; set; } = 180f;

    
    [GimmickData("멈췄다가 다시 돌아가는 시간")]
    [field: SerializeField]
    public float SphereStopTime { get; set; } = 30f;

    public override void SaveGimmickData(in LDMapData _mapData)
    {

        var sdBlueCristalGimmickData = new LDBlueCristalGimmickData();

        sdBlueCristalGimmickData.Position = trGimmick.position;
        sdBlueCristalGimmickData.Rotation = trGimmick.rotation.eulerAngles;
        sdBlueCristalGimmickData.Scale = trGimmick.localScale;
        sdBlueCristalGimmickData.Address = address;

        sdBlueCristalGimmickData.CristalType = CristalType;
        sdBlueCristalGimmickData.BlinkShowTime = BlinkShowTime;
        sdBlueCristalGimmickData.BlinkHideTime = BlinkHideTime;

        sdBlueCristalGimmickData.MoveMoveTime = MoveMoveTime;
        sdBlueCristalGimmickData.ChangeValueTime = ChangeValueTime;
        sdBlueCristalGimmickData.RandomMinValue = RandomMinValue;
        sdBlueCristalGimmickData.RandomMaxValue = RandomMaxValue;

        sdBlueCristalGimmickData.SphereSize = SphereSize;
        sdBlueCristalGimmickData.SphereMoveTime = SphereMoveTime;
        sdBlueCristalGimmickData.SphereRotateSpeed = SphereRotateSpeed;


        foreach (var kvPoint in DictPoint)
        {
            sdBlueCristalGimmickData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        _mapData.BlueCristalGimmickDataList.Add(sdBlueCristalGimmickData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        var sdBlueCristalGimmickData = (LDBlueCristalGimmickData)_ldData;

        trGimmick.position = sdBlueCristalGimmickData.Position;
        trGimmick.rotation = Quaternion.Euler(sdBlueCristalGimmickData.Rotation);
        trGimmick.localScale = sdBlueCristalGimmickData.Scale;

        CristalType = sdBlueCristalGimmickData.CristalType;
        BlinkShowTime = sdBlueCristalGimmickData.BlinkShowTime;
        BlinkHideTime = sdBlueCristalGimmickData.BlinkHideTime;

        MoveMoveTime = sdBlueCristalGimmickData.MoveMoveTime;
        ChangeValueTime = sdBlueCristalGimmickData.ChangeValueTime;
        RandomMinValue = sdBlueCristalGimmickData.RandomMinValue;
        RandomMaxValue = sdBlueCristalGimmickData.RandomMaxValue;

        SphereSize = sdBlueCristalGimmickData.SphereSize;
        SphereMoveTime = sdBlueCristalGimmickData.SphereMoveTime;
        SphereRotateSpeed = sdBlueCristalGimmickData.SphereRotateSpeed;


        foreach (var kv in sdBlueCristalGimmickData.DictPoint)
        {
            DictPoint[kv.Key].position = kv.Value;
        }
    }
}
