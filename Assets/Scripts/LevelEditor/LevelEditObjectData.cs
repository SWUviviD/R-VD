using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditObjectData : GimmickDataBase
{
    public override void SaveGimmickData(in LDMapData _mapData)
    {
        base.SaveGimmickData(_mapData);

        var sdLevelEditObject = new LDLevelEditObject();

        sdLevelEditObject.Position = trGimmick.position;
        sdLevelEditObject.Rotation = trGimmick.rotation.eulerAngles;
        sdLevelEditObject.Scale = trGimmick.localScale;
        sdLevelEditObject.Address = address;

        _mapData.LevelEditObjectList.Add(sdLevelEditObject);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var ldLevelEditObjectData = (LDLevelEditObject)_ldData;

        trGimmick.position = ldLevelEditObjectData.Position;
        trGimmick.rotation = Quaternion.Euler(ldLevelEditObjectData.Rotation);
        trGimmick.localScale = ldLevelEditObjectData.Scale;
    }
}
