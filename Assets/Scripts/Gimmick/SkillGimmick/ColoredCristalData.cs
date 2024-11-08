using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredCristalData : CristalData
{
    [GimmickData("크리스탈 이동 속도 바뀌는 주기")]
    [field: SerializeField]
    public float ChangeValueTime { get; set; } = 10f;


    [GimmickData("크리스탈 이동 속도 최소 값")]
    [field: SerializeField]
    public float RandomMinValue { get; set; } = 1f;


    [GimmickData("크리스탈 이동 속도 최대 값")]
    [field: SerializeField]
    public float RandomMaxValue { get; set; } = 3f;
}
