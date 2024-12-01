using LocalData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGimmickData : GimmickDataBase
{
    [GimmickData("추격 범위")]
    [field :SerializeField]
    public Vector3 ChaseFloorScale { get; set; } = Vector3.one;

    
    [GimmickData("작은 별이 연속으로 떨어지는 갯수")]
    [field :SerializeField]
    public int SmallStarCount { get; set; } = 3;

    [GimmickData("작은 별이 떨어지기 시작한 시간")]
    [field: SerializeField]
    public float SmallStarStartDelay { get; set; } = 0.5f;

    [GimmickData("작은 별이 연속으로 떨어질 때 그 간격 시간")]
    [field :SerializeField]
    public float SmallStarFallingIntervalTime{ get; set; } = 0.5f;

    [GimmickData("작은 별들이 떨어진 후 쿨타임")]
    [field :SerializeField]
    public float SmallStarCoolTime{ get; set; } = 3f;

    

    [GimmickData("중간 별이 연속으로 떨어지는 갯수")]
    [field :SerializeField]
    public int MediumStarCount { get; set; } = 2;

    [GimmickData("중간 별이 떨어지기 시작한 시간")]
    [field: SerializeField]
    public float MediumStarStartDelay { get; set; } = 2.5f;

    [GimmickData("중간 별이 연속으로 떨어질 때 그 간격 시간")]
    [field :SerializeField]
    public float MediumStarFallingIntervalTime { get; set; } = 1f;

    [GimmickData("중간 별들이 떨어진 후 쿨타임")]
    [field :SerializeField]
    public float MediumStarCoolTime { get; set; } = 4f;

    

    [GimmickData("큰 별이 연속으로 떨어지는 갯수")]
    [field :SerializeField]
    public int BigStarCount { get; set; } = 1;

    [GimmickData("큰 별이 떨어지기 시작한 시간")]
    [field: SerializeField]
    public float BigStarStartDelay { get; set; } = 5f;

    [GimmickData("큰 별이 연속으로 떨어질 때 그 간격 시간")]
    [field :SerializeField]
    public float BigStarFallingIntervalTime { get; set; } = 0f;

    [GimmickData("큰 별들이 떨어진 후 쿨타임")]
    [field :SerializeField]
    public float BigStarCoolTime { get; set; } = 5f;



    public override void SaveGimmickData(in LDMapData _mapData)
    {
        var sdChasingData = new LDChasingGimmickData();

        sdChasingData.ChaseFloorScale = ChaseFloorScale;

        sdChasingData.SmallStarCount = SmallStarCount;
        sdChasingData.SmallStarStartDelay = SmallStarStartDelay;
        sdChasingData.SmallStarFallingIntervalTime = SmallStarFallingIntervalTime;
        sdChasingData.SmallStarCoolTime = SmallStarCoolTime;

        sdChasingData.MediumStarCount = MediumStarCount;
        sdChasingData.MediumStarStartDelay = MediumStarStartDelay;
        sdChasingData.MediumStarFallingIntervalTime = MediumStarFallingIntervalTime;
        sdChasingData.MediumStarCoolTime = MediumStarCoolTime;

        sdChasingData.BigStarCount = BigStarCount;
        sdChasingData.BigStarStartDelay = BigStarStartDelay;
        sdChasingData.BigStarFallingIntervalTime = BigStarFallingIntervalTime;
        sdChasingData.BigStarCoolTime = BigStarCoolTime;

        foreach(var kvPoint in DictPoint)
        {
            sdChasingData.DictPoint.Add(kvPoint.Key, kvPoint.Value.position);
        }

        sdChasingData.Position = trGimmick.position;
        sdChasingData.Rotation = trGimmick.rotation.eulerAngles;
        sdChasingData.Scale = trGimmick.localScale;
        sdChasingData.Address = address;

        _mapData.ChasingGimmickDataList.Add(sdChasingData);
    }

    public override void Set(LDGimmickDataBase _ldData)
    {
        base.Set(_ldData);

        var sdChasingData = (LDChasingGimmickData)_ldData;

        trGimmick.position = sdChasingData.Position;
        trGimmick.rotation = Quaternion.Euler(sdChasingData.Rotation);
        trGimmick.localScale = sdChasingData.Scale;

        ChaseFloorScale = sdChasingData.ChaseFloorScale;

        SmallStarCount = sdChasingData.SmallStarCount;
        SmallStarStartDelay = sdChasingData.SmallStarStartDelay;
        SmallStarFallingIntervalTime = sdChasingData.SmallStarFallingIntervalTime;
        SmallStarCoolTime = sdChasingData.SmallStarCoolTime;

        MediumStarCount = sdChasingData.MediumStarCount;
        MediumStarStartDelay = sdChasingData.MediumStarStartDelay;
        MediumStarFallingIntervalTime = sdChasingData.MediumStarFallingIntervalTime;
        MediumStarCoolTime = sdChasingData.MediumStarCoolTime;

        BigStarCount = sdChasingData.BigStarCount;
        BigStarStartDelay = sdChasingData.BigStarStartDelay;
        BigStarFallingIntervalTime = sdChasingData.BigStarFallingIntervalTime;
        BigStarCoolTime = sdChasingData.BigStarCoolTime;
    }
}
