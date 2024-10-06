using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkBoardData : GimmickDataBase
{
    [field: Header("빙판의 개수")]
    [field: SerializeField]
    public int BoardCount { get; set; } = 3;

    [field: Header("빙판의 크기")]
    [field: SerializeField]
    public float BoardSize { get; set; } = 1f;

    [field: Header("사라졌다가 다시 돌아오는데 걸리는 시간")]
    [field: SerializeField]
    public float BlinkTime { get; set; } = 0.5f;

    [field: Header("빙판이 유지되는 시간")]
    [field: SerializeField]
    public float DurationTime { get; set; } = 3f;

    [field: Header("다음 빙판이 사라지는 텀")]
    [field: SerializeField]
    public float NextBlinkTime { get; set; } = 0.25f;
}
