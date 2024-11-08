using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCristalGimmickData : ColoredCristalData
{
    [GimmickData("공 크기")]
    [field: SerializeField]
    public float SphereSize { get; set; } = 10f;
    
    [GimmickData("공이 특정 목적지까지 도달하는 시간")]
    [field: SerializeField]
    public float SphereMoveTime { get; set; } = 16f;
    
    [GimmickData("공 회전 속도")]
    [field: SerializeField]
    public float SphereRotateSpeed { get; set; } = 180f;

    
    [GimmickData("멈췄다가 다시 돌아가는 시간")]
    [field: SerializeField]
    public float SphereStopTime { get; set; } = 30f;
}
