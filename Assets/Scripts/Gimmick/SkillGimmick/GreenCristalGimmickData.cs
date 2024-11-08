using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCristalGimmickData : ColoredCristalData
{

    [GimmickData("발판 갯수")]
    [field: SerializeField]
    public int PlateCount { get; set; } = 3;

    [GimmickData("별판이 나타나는 시간")]
    [field: SerializeField]
    public float GimmickAppearTime { get; set; } = 0.3f;

    [GimmickData("별판이 유지되는 시간")]
    [field: SerializeField]
    public float GimmickShowTime { get; set; } = 3f;

    [GimmickData("다음 별판이 보여지는 시간")]
    [field: SerializeField]
    public float NextPlateShowTime { get; set; } = 2f;
    
    [GimmickData("별판이 사라지는 시간")]
    [field: SerializeField]
    public float GimmickDissappearTime{ get; set; } = 0.3f;
    
    [GimmickData("별판이 소멸 시간")]
    [field: SerializeField]
    public float GimmickHideTime{ get; set; } = 3f;
}
