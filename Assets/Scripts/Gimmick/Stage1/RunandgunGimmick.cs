using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunandgunGimmick : GimmickBase<RunandgunGimmickData>
{
    [SerializeField] private PlayerStatus playerStatus; // 플레이어 상태를 관리하는 스크립트 참조


    /// <summary>
    /// 변수 선언
    /// </summary>

    /// <summary>
    /// 상속
    /// </summary>
    protected override void Init()
    {
        // 추가 작업 없음 
    }

    public override void SetGimmick()
    {
        
    }
}
