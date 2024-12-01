using LocalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleData : GimmickDataBase
{
    [GimmickData("방울이 사라지는 시간")]
    [field: SerializeField]
    public float PopOffsetTime {  get; set; }

    [GimmickData("방울이 다시 보여지는 시간")]
    [field: SerializeField]
    public float ResetOffsetTime { get; set; }

    [GimmickData("방울의 크기")]
    [field: SerializeField]
    public float BubbleSize { get; set; }

    [GimmickData("방울에 의해 점프 되는 힘")]
    [field: SerializeField]
    public float JumpForce { get; set; } = 150f;

    public override void SaveGimmickData(in LDMapData _mapData)
    {
        base.SaveGimmickData(_mapData);

        var sdBubbleData = new LDBubbleData();

        sdBubbleData.Position = trGimmick.position;
        sdBubbleData.Rotation = trGimmick.rotation.eulerAngles;
        sdBubbleData.Scale = trGimmick.localScale;
        sdBubbleData.Address = address;

        sdBubbleData.PopOffsetTime = PopOffsetTime;
        sdBubbleData.ResetOffsetTime = ResetOffsetTime;
        sdBubbleData.BubbleSize = BubbleSize;
        sdBubbleData.JumpForce = JumpForce;

        foreach(var p in DictPoint)
        {
            sdBubbleData.DictPoint.Add(p.Key, p.Value.position);
        }

        _mapData.BubbleDataList.Add(sdBubbleData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdBubbleData = (LDBubbleData)_ldData;

        trGimmick.position = sdBubbleData.Position;
        trGimmick.rotation = Quaternion.Euler(sdBubbleData.Rotation);
        trGimmick.localScale = sdBubbleData.Scale;

        PopOffsetTime = sdBubbleData.PopOffsetTime;
        ResetOffsetTime = sdBubbleData.ResetOffsetTime;
        BubbleSize = sdBubbleData.BubbleSize;
        JumpForce = sdBubbleData.JumpForce;
    }

}
