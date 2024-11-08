using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalData : GimmickDataBase
{
    [GimmickData("크리스탈 타입")]
    [field: SerializeField]
    public int CristalType { get; set; }



    [GimmickData("깜빡이는 크리스탈: 보여지는 시간")]
    [field: SerializeField]
    public float BlinkShowTime { get; set; } = 3f;

    [GimmickData("깜빡이는 크리스탈: 사라지는 시간")]
    [field: SerializeField]
    public float BlinkHideTime { get; set; } = 3f;


    [GimmickData("움직이는 크리스탈: 좌에서 우로 이동하는 시간")]
    [field: SerializeField]
    public float MoveMoveTime { get; set; } = 3f;
}
