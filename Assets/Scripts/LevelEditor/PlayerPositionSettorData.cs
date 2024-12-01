using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSettorData : GimmickDataBase
{
    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdPlayerSpawnObject = new LDPlayerPositionSettor();

        sdPlayerSpawnObject.Position = trGimmick.position;
        sdPlayerSpawnObject.Rotation = trGimmick.rotation.eulerAngles;
        sdPlayerSpawnObject.Scale = trGimmick.localScale;
        sdPlayerSpawnObject.Address = address;

        _mapData.PlayerPositionSettor = sdPlayerSpawnObject;
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        var ldPlayerPositionSettor = (LDPlayerPositionSettor)_ldData;

        trGimmick.position = ldPlayerPositionSettor.Position;
        trGimmick.rotation = Quaternion.Euler(ldPlayerPositionSettor.Rotation);
        trGimmick.localScale = ldPlayerPositionSettor.Scale;
    }
}
