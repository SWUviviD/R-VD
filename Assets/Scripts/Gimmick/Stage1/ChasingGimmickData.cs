using LocalData;
using System;
using UnityEngine;

public class ChasingGimmickData : GimmickDataBase
{
    [GimmickData("별똥별이 떨어지는 시간 (개당)")]
    [field: SerializeField]
    public float StarFallSpeed { get; set; } = 2f;

    [GimmickData("별똥별의 리스폰 간격")]
    [field: SerializeField]
    public float StarShowInterval { get; set; } = 2f;

    [GimmickData("충돌 시 플레이어의 데미지량")]
    [field: SerializeField]
    public float PlayerDamage { get; set; } = 20;
}
