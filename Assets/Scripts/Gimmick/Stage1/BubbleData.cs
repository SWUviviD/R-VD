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
}
