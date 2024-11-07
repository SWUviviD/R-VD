using System;
using System.Collections;
using System.Collections.Generic;
using LocalData;
using UnityEngine;

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
    
    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdBlinkBoardData = new LDBlinkBoardData();
        
        sdBlinkBoardData.Position = trGimmick.position;
        sdBlinkBoardData.Rotation = trGimmick.rotation.eulerAngles;
        sdBlinkBoardData.Scale = trGimmick.localScale;
        sdBlinkBoardData.Address = address;
        
        foreach (var kvPoint in DictPoint)
        {
            sdBlinkBoardData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdBlinkBoardData.BoardCount = BoardCount;
        sdBlinkBoardData.BoardSize = BoardSize;
        sdBlinkBoardData.BlinkTime = BlinkTime;
        sdBlinkBoardData.DurationTime = DurationTime;
        sdBlinkBoardData.NextBlinkTime = NextBlinkTime;
        
        _mapData.BlinkBoardDataList.Add(sdBlinkBoardData);
    }

    public void Set(LDBlinkBoardData _ldData)
    {
        foreach (var kv in _ldData.DictPoint)
        {
            DictPoint[kv.Key].position = kv.Value;
        }

        trGimmick.position = _ldData.Position;
        trGimmick.rotation = Quaternion.Euler(_ldData.Rotation);
        trGimmick.localScale = _ldData.Scale;
        
        BoardCount = _ldData.BoardCount;
        BoardSize = _ldData.BoardSize;
        BlinkTime = _ldData.BlinkTime;
        DurationTime = _ldData.DurationTime;
        NextBlinkTime = _ldData.NextBlinkTime;
    }
}
