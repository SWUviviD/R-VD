using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCristalGimmickData : ColoredCristalData
{

    [GimmickData("발판 갯수")]
    [field: SerializeField]
    public int PlateCount { get; set; } = 3;

    [GimmickData("별판이 나타나는 시간")]
    [field: SerializeField]
    public float GimmickAppearTime { get; set; } = 0.3f;

    [GimmickData("별판이 유지되는 시간")]
    [field: SerializeField]
    public float GimmickShowTime { get; set; } = 3f;

    [GimmickData("다음 별판이 보여지는 시간")]
    [field: SerializeField]
    public float NextPlateShowTime { get; set; } = 2f;
    
    [GimmickData("별판이 사라지는 시간")]
    [field: SerializeField]
    public float GimmickDissappearTime{ get; set; } = 0.3f;
    
    [GimmickData("별판이 소멸 시간")]
    [field: SerializeField]
    public float GimmickHideTime{ get; set; } = 3f;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdGreenCristalGimmickData = new LDGreenCristalGimmickData();

        sdGreenCristalGimmickData.Position = trGimmick.position;
        sdGreenCristalGimmickData.Rotation = trGimmick.rotation.eulerAngles;
        sdGreenCristalGimmickData.Scale = trGimmick.localScale;
        sdGreenCristalGimmickData.Address = address;

        sdGreenCristalGimmickData.CristalType = CristalType;
        sdGreenCristalGimmickData.BlinkShowTime = BlinkShowTime;
        sdGreenCristalGimmickData.BlinkHideTime = BlinkHideTime;

        sdGreenCristalGimmickData.MoveMoveTime = MoveMoveTime;
        sdGreenCristalGimmickData.ChangeValueTime = ChangeValueTime;
        sdGreenCristalGimmickData.RandomMinValue = RandomMinValue;
        sdGreenCristalGimmickData.RandomMaxValue = RandomMaxValue;

        sdGreenCristalGimmickData.PlateCount = PlateCount;
        sdGreenCristalGimmickData.GimmickAppearTime = GimmickAppearTime;
        sdGreenCristalGimmickData.GimmickShowTime = GimmickShowTime;
        sdGreenCristalGimmickData.NextPlateShowTime = NextPlateShowTime;
        sdGreenCristalGimmickData.GimmickDissappearTime = GimmickDissappearTime;
        sdGreenCristalGimmickData.GimmickHideTime = GimmickHideTime;


        foreach (var kvPoint in DictPoint)
        {
            sdGreenCristalGimmickData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        _mapData.GreenCristalGimmickDataList.Add(sdGreenCristalGimmickData);

    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        var sdGreenCristalGimmickData = (LDGreenCristalGimmickData)_ldData;

        trGimmick.position = sdGreenCristalGimmickData.Position;
        trGimmick.rotation = Quaternion.Euler(sdGreenCristalGimmickData.Rotation);
        trGimmick.localScale = sdGreenCristalGimmickData.Scale;

        CristalType = sdGreenCristalGimmickData.CristalType;
        BlinkShowTime = sdGreenCristalGimmickData.BlinkShowTime;
        BlinkHideTime = sdGreenCristalGimmickData.BlinkHideTime;

        MoveMoveTime = sdGreenCristalGimmickData.MoveMoveTime;
        ChangeValueTime = sdGreenCristalGimmickData.ChangeValueTime;
        RandomMinValue = sdGreenCristalGimmickData.RandomMinValue;
        RandomMaxValue = sdGreenCristalGimmickData.RandomMaxValue;

        PlateCount = sdGreenCristalGimmickData.PlateCount;
        GimmickAppearTime = sdGreenCristalGimmickData.GimmickAppearTime;
        GimmickShowTime = sdGreenCristalGimmickData.GimmickShowTime;
        NextPlateShowTime = sdGreenCristalGimmickData.NextPlateShowTime;
        GimmickDissappearTime = sdGreenCristalGimmickData.GimmickDissappearTime;
        GimmickHideTime = sdGreenCristalGimmickData.GimmickHideTime;


        foreach (var kv in sdGreenCristalGimmickData.DictPoint)
        {
            DictPoint[kv.Key].position = kv.Value;
        }
    }
}
