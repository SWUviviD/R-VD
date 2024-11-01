using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunandgunGimmickData : GimmickDataBase
{
    [GimmickData("Tick 시간당 감소하는 체력량")]
    [field: SerializeField]
    public float DamageAmount { get; set; } = 10f; // 데미지 존에서 틱당 감소되는 체력 양

    [GimmickData("회복 구간에서 회복되는 체력량")]
    [field: SerializeField]
    public float HealAmount { get; set; } = 100f; // 힐 존에서 회복되는 체력 양

    [GimmickData("체력을 감소시킬 기준시간(초)")]
    [field: SerializeField]
    public float DamageTickInterval { get; set; } = 2f; // 데미지 틱 간격 시간 (초)
}
