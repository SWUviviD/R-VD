using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RolerGimmickData : GimmickDataBase
{
    [GimmickData("한 바퀴를 회전할 시간 (2.5초-5초)")]
    [field: SerializeField]
    public float rotationDuration { get; set; } = 2.5f;

    [GimmickData("회전 방향 결정 (체크 시 시계방향)")]
    [field: SerializeField]
    public bool rotateClockwise { get; set; } = true;
}
