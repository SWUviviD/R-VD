using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyGimmickData : GimmickDataBase
{
    [GimmickData("오브젝트 가시화 시간")]
    [field: SerializeField]
    public float VisibleDuration { get; set; } = 4f;

    [GimmickData("넉백 세기 (기본 120f)")]
    [field: SerializeField]
    public float KnockbackForce { get; set; } = 120f;

    [GimmickData("데미지량 (기본 2)")]
    [field: SerializeField]
    public int Damage { get; set; } = 2;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdGalaxyData = new LDGalaxyGimmickData();

        sdGalaxyData.VisibleDuration = VisibleDuration;
        sdGalaxyData.KnockbackForce = KnockbackForce;
        sdGalaxyData.Damage = Damage;

        sdGalaxyData.Position = trGimmick.position;
        sdGalaxyData.Rotation = trGimmick.rotation.eulerAngles;
        sdGalaxyData.Scale = trGimmick.localScale;

        _mapData.GalaxyGimmickDataList.Add(sdGalaxyData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdGalaxyData = (LDGalaxyGimmickData)_ldData;

        trGimmick.position = sdGalaxyData.Position;
        trGimmick.rotation = Quaternion.Euler(sdGalaxyData.Rotation);
        trGimmick.localScale = sdGalaxyData.Scale;

        VisibleDuration = sdGalaxyData.VisibleDuration;
        KnockbackForce = sdGalaxyData.KnockbackForce;
        Damage = sdGalaxyData.Damage;
    }
}
