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

    [GimmickData("기믹 크기")]
    [field: SerializeField]
    public float Size { get; set; } = 1f;

    [GimmickData("오브젝트 가시화 시간")]
    [field: SerializeField]
    public float RotationSpeed { get; set; } = 10f;

    [GimmickData("오브젝트 가시화 시간")]
    [field: SerializeField]
    public float MinDisappearTime { get; set; } = 2f;

    [GimmickData("오브젝트 가시화 시간")]
    [field: SerializeField]
    public float MaxDisappearTime { get; set; } = 5f;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdGalaxyData = new LDGalaxyGimmickData();

        sdGalaxyData.VisibleDuration = VisibleDuration;
        sdGalaxyData.KnockbackForce = KnockbackForce;
        sdGalaxyData.Damage = Damage;
        sdGalaxyData.Size = Size;

        sdGalaxyData.RotationSpeed = RotationSpeed;
        sdGalaxyData.MinDisappearTime = MinDisappearTime;
        sdGalaxyData.MaxDisappearTime = MaxDisappearTime;

        foreach (var kvPoint in DictPoint)
        {
            sdGalaxyData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdGalaxyData.Position = trGimmick.position;
        sdGalaxyData.Rotation = trGimmick.rotation.eulerAngles;
        sdGalaxyData.Scale = trGimmick.localScale;
        sdGalaxyData.Address = address;

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

        Size = sdGalaxyData.Size;

        RotationSpeed = sdGalaxyData.RotationSpeed;
        MinDisappearTime = sdGalaxyData.MinDisappearTime;
        MaxDisappearTime = sdGalaxyData.MaxDisappearTime;
    }
}
