using Cinemachine.Utility;
using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancePlateData : GimmickDataBase
{
    [GimmickData("판의 반지름")]
    [field: SerializeField]
    public float Radius { get; set; }

    [GimmickData("판의 반지름")]
    [field: SerializeField]
    public float ReturnToNormalTime { get; set; }

    [GimmickData("1단계 초당 기울기")]
    [field: SerializeField]
    public float Level1_Roate { get; set; }

    [GimmickData("2단계 초당 기울기")]
    [field: SerializeField]
    public float Level2_Roate { get; set; }

    [GimmickData("3단계 초당 기울기")]
    [field: SerializeField]
    public float Level3_Roate { get; set; }


    public override void SaveGimmickData(in LDMapData _mapData)
    {
        base.SaveGimmickData(_mapData);

        var sdBalancePlateData = new LDBalancePlateData();

        sdBalancePlateData.Position = trGimmick.position;
        sdBalancePlateData.Rotation = trGimmick.rotation.eulerAngles;
        sdBalancePlateData.Scale = trGimmick.localScale;
        sdBalancePlateData.Address = address;

        sdBalancePlateData.Radius = Radius;
        sdBalancePlateData.ReturnToNormalTime = ReturnToNormalTime;
        sdBalancePlateData.Level1_Roate = Level1_Roate;
        sdBalancePlateData.Level2_Roate = Level2_Roate;
        sdBalancePlateData.Level3_Roate = Level3_Roate;

        foreach(var p in DictPoint)
        {
            sdBalancePlateData.DictPoint.Add(p.Key, p.Value.position);
        }

        _mapData.BalancePlateDataList.Add(sdBalancePlateData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdBalancePlateData = (LDBalancePlateData)_ldData;

        trGimmick.position = sdBalancePlateData.Position;
        trGimmick.rotation = Quaternion.Euler(sdBalancePlateData.Rotation);
        trGimmick.localScale = sdBalancePlateData.Scale;

        Radius = sdBalancePlateData.Radius;
        ReturnToNormalTime = sdBalancePlateData.ReturnToNormalTime;
        Level1_Roate = sdBalancePlateData.Level1_Roate;
        Level2_Roate = sdBalancePlateData.Level2_Roate;
        Level3_Roate = sdBalancePlateData.Level3_Roate;

        foreach(var p in sdBalancePlateData.DictPoint)
        {
            DictPoint[p.Key].position = p.Value;
        }
    }
}
