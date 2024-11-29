using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalData : GimmickDataBase
{
    [GimmickData("크리스탈 타입")]
    [field: SerializeField]
    public int CristalType { get; set; }



    [GimmickData("깜빡이는 크리스탈: 보여지는 시간")]
    [field: SerializeField]
    public float BlinkShowTime { get; set; } = 3f;

    [GimmickData("깜빡이는 크리스탈: 사라지는 시간")]
    [field: SerializeField]
    public float BlinkHideTime { get; set; } = 3f;


    [GimmickData("움직이는 크리스탈: 좌에서 우로 이동하는 시간")]
    [field: SerializeField]
    public float MoveMoveTime { get; set; } = 3f;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        base.SaveGimmickData(_mapData);

        var sdCristalData = new LDCristalData();

        sdCristalData.Position = trGimmick.position;
        sdCristalData.Rotation = trGimmick.rotation.eulerAngles;
        sdCristalData.Scale = trGimmick.localScale;
        sdCristalData.Address = address;

        foreach (var kvPoint in DictPoint)
        {
            sdCristalData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdCristalData.CristalType = CristalType;
        sdCristalData.BlinkShowTime = BlinkShowTime;
        sdCristalData.BlinkHideTime = BlinkHideTime;
        sdCristalData.MoveMoveTime = MoveMoveTime;

        _mapData.CristalGimmickDataList.Add(sdCristalData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var ldCristalData = (LDCristalData)_ldData;

        foreach (var kv in ldCristalData.DictPoint)
        {
            DictPoint[kv.Key].position = kv.Value;
        }

        trGimmick.position = ldCristalData.Position;
        trGimmick.rotation = Quaternion.Euler(ldCristalData.Rotation);
        trGimmick.localScale = ldCristalData.Scale;

        CristalType = ldCristalData.CristalType;
        BlinkShowTime = ldCristalData.BlinkShowTime;
        BlinkHideTime = ldCristalData.BlinkHideTime;
        MoveMoveTime = ldCristalData.MoveMoveTime;

    }
}
