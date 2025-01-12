using LocalData;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WaterBlockData : GimmickDataBase
{
    [GimmickData("물을 담을 수 있는 최대 용량")]
    [field: SerializeField]
    public int maxWaterCapacity = 3;

    [GimmickData("실제 물 저장 용량")]
    [field: SerializeField]
    public int WaterUsage = 3;

    [GimmickData("블록 메테리얼")]
    [field: SerializeField]
    public Material[] blockMaterials;


    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdWaterBlockData = new LDWaterBlockData();

        sdWaterBlockData.maxWaterCapacity = maxWaterCapacity;
        sdWaterBlockData.maxWaterUsage = WaterUsage;
        sdWaterBlockData.blockMaterials = blockMaterials;

        foreach (var kvPoint in DictPoint)
        {
            sdWaterBlockData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdWaterBlockData.Position = trGimmick.position;
        sdWaterBlockData.Rotation = trGimmick.rotation.eulerAngles;
        sdWaterBlockData.Scale = trGimmick.localScale;
        sdWaterBlockData.Address = address;

        _mapData.WaterBlockDataList.Add(sdWaterBlockData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdWaterBlockData = (LDWaterBlockData)_ldData;

        trGimmick.position = sdWaterBlockData.Position;
        trGimmick.rotation = Quaternion.Euler(sdWaterBlockData.Rotation);
        trGimmick.localScale = sdWaterBlockData.Scale;

        WaterUsage = sdWaterBlockData.maxWaterUsage;
        maxWaterCapacity = sdWaterBlockData.maxWaterCapacity;
        blockMaterials = sdWaterBlockData.blockMaterials;
    }
}
