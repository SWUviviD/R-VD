using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearPointData : GimmickDataBase
{
    [GimmickData("구역 크기")]
    [field: SerializeField]
    public Vector3 AreaScale {  get; set; } = Vector3.one;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdStageClearData = new LDStageClearPointData();

        sdStageClearData.Position = trGimmick.position;
        sdStageClearData.Rotation = trGimmick.rotation.eulerAngles;
        sdStageClearData.Scale = trGimmick.localScale;
        sdStageClearData.Address = address;

        sdStageClearData.AreaScale = AreaScale;

        _mapData.StageClearPoint = sdStageClearData;
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        var ldStageClearPoint = (LDStageClearPointData)_ldData;

        trGimmick.position = ldStageClearPoint.Position;
        trGimmick.rotation = Quaternion.Euler(ldStageClearPoint.Rotation);
        trGimmick.localScale = ldStageClearPoint.Scale;

        AreaScale = ldStageClearPoint.AreaScale;
    }
}
