using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancePlateData : GimmickDataBase
{
    [GimmickData("판의 반지름")]
    [field: SerializeField]
    public float Radius;

    [GimmickData("판의 반지름")]
    [field: SerializeField]
    public float ReturnToNormalTime;

    [GimmickData("1단계 초당 기울기")]
    [field: SerializeField]
    public float Level1_Roate;
    
    [GimmickData("2단계 초당 기울기")]
    [field: SerializeField]
    public float Level2_Roate;
    
    [GimmickData("3단계 초당 기울기")]
    [field: SerializeField]
    public float Level3_Roate;

}
