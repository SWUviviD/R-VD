using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWallData : GimmickDataBase
{
    [GimmickData("블록 메테리얼")]
    [field: SerializeField]
    public Material[] blockMaterials;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdWaterWallData = new LDWaterWallData();

        sdWaterWallData.blockMaterials = blockMaterials;

        foreach (var kvPoint in DictPoint)
        {
            sdWaterWallData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdWaterWallData.Position = trGimmick.position;
        sdWaterWallData.Rotation = trGimmick.rotation.eulerAngles;
        sdWaterWallData.Scale = trGimmick.localScale;
        sdWaterWallData.Address = address;

        _mapData.WaterWallDataList.Add(sdWaterWallData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdWaterWallData = (LDWaterWallData)_ldData;

        trGimmick.position = sdWaterWallData.Position;
        trGimmick.rotation = Quaternion.Euler(sdWaterWallData.Rotation);
        trGimmick.localScale = sdWaterWallData.Scale;

        blockMaterials = sdWaterWallData.blockMaterials;
    }
}
