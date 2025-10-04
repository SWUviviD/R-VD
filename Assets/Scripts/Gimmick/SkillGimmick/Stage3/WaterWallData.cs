using LocalData;
using UnityEngine;

public class WaterWallData : GimmickDataBase
{
    [GimmickData("프리팹 배열")]
    [field: SerializeField]
    public GameObject[] WaterWallPrefabs { get; private set; }

    [GimmickData("회전 보정값")]
    [field: SerializeField]
    public Vector3 RotationOffset { get; private set; } = Vector3.zero;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdWaterWallData = new LDWaterWallData();

        sdWaterWallData.Position = trGimmick.position;
        sdWaterWallData.Rotation = trGimmick.rotation.eulerAngles;
        sdWaterWallData.Scale = trGimmick.localScale;
        sdWaterWallData.Address = address;
        sdWaterWallData.RotationOffset = RotationOffset;

        _mapData.WaterWallDataList.Add(sdWaterWallData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdWaterWallData = (LDWaterWallData)_ldData;

        trGimmick.position = sdWaterWallData.Position;
        trGimmick.rotation = Quaternion.Euler(sdWaterWallData.Rotation);
        trGimmick.localScale = sdWaterWallData.Scale;
        RotationOffset = sdWaterWallData.RotationOffset;
    }
}
