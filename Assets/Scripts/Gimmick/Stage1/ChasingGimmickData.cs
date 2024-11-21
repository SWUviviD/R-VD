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
    public float Damage { get; set; } = 20;

    [GimmickData("넉백 세기 (기본 120f)")]
    [field: SerializeField]
    public float KnockbackForce { get; set; } = 120f;

    [GimmickData("! 건들지 말아주세요 !\n(별똥별 prefab 인식용 배열)")]
    [field: SerializeField]
    public List<ChasingStarProp> starListPrefab;
}
