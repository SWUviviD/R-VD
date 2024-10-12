using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickDataAttribute : Attribute
{
    public string Desc { get; private set; }
    public GimmickDataAttribute(string _desc)
    {
        Desc = _desc;
    }
}

public class BlinkBoardData : GimmickDataBase
{
    [GimmickData("빙판의 개수")]
    [field: SerializeField]
    public int BoardCount { get; set; } = 3;

    [GimmickData("빙판의 크기")]
    [field: SerializeField]
    public float BoardSize { get; set; } = 1f;

    [GimmickData("사라졌다가 다시 돌아오는데 걸리는 시간")]
    [field: SerializeField]
    public float BlinkTime { get; set; } = 0.5f;

    [GimmickData("빙판이 유지되는 시간")]
    [field: SerializeField]
    public float DurationTime { get; set; } = 3f;

    [GimmickData("다음 빙판이 사라지는 텀")]
    [field: SerializeField]
    public float NextBlinkTime { get; set; } = 0.25f;
}
