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

    [GimmickData("데미지량 (기본 20f)")]
    [field: SerializeField]
    public float Damage { get; set; } = 20f;
}
