using LocalData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGimmickData : GimmickDataBase
{
    [GimmickData("별똥별이 떨어지는 시간(s)")]
    [field: SerializeField]
    public float StarFallSpeed { get; set; } = 4f;

    [GimmickData("별똥별의 리스폰 간격(s)")]
    [field: SerializeField]
    public float ResponeTime { get; set; } = 2f;

    [GimmickData("전체 별똥별 개수(1~4개)")]
    [field: SerializeField]
    public int TotalNum { get; set; } = 4;

    [GimmickData("충돌 시 플레이어의 데미지량")]
    [field: SerializeField]
    public int Damage { get; set; } = 2;

    [GimmickData("넉백 세기 (기본 120f)")]
    [field: SerializeField]
    public float KnockbackForce { get; set; } = 120f;

    [GimmickData("! 건들지 말아주세요 !\n(별똥별 prefab 인식용 배열)")]
    [field: SerializeField]
    public List<ChasingStarProp> starListPrefab;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdChasingData = new LDChasingGimmickData();

        sdChasingData.StarFallSpeed = StarFallSpeed;
        sdChasingData.ResponeTime = ResponeTime;
        sdChasingData.TotalNum = TotalNum;
        sdChasingData.Damage = Damage;
        sdChasingData.KnockbackForce = KnockbackForce;

        foreach(var kvPoint in DictPoint)
        {
            sdChasingData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdChasingData.Position = trGimmick.position;
        sdChasingData.Rotation = trGimmick.rotation.eulerAngles;
        sdChasingData.Scale = trGimmick.localScale;

        sdChasingData.starListPrefab = new List<ChasingStarProp>(starListPrefab);

        _mapData.ChasingGimmickDataList.Add(sdChasingData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdChasingData = (LDChasingGimmickData)_ldData;

        trGimmick.position = sdChasingData.Position;
        trGimmick.rotation = Quaternion.Euler(sdChasingData.Rotation);
        trGimmick.localScale = sdChasingData.Scale;

        StarFallSpeed = sdChasingData.StarFallSpeed;
        ResponeTime = sdChasingData.ResponeTime;
        TotalNum = sdChasingData.TotalNum;
        Damage = sdChasingData.Damage;
        KnockbackForce = sdChasingData.KnockbackForce;

        starListPrefab = new List<ChasingStarProp>(sdChasingData.starListPrefab);
    }
}
