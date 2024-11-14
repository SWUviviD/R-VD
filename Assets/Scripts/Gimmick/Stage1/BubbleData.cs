using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleData : GimmickDataBase
{
    [GimmickData("방울이 사라지는 시간")]
    [field: SerializeField]
    public float PopOffsetTime;

    [GimmickData("방울이 다시 보여지는 시간")]
    [field: SerializeField]
    public float ResetOffsetTime;

    [GimmickData("방울에 의해 점프 되는 힘")]
    [field: SerializeField]
    [Range(50f, float.MaxValue)]
    public float JumpForce = 150f;
}
