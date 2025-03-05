using System;
using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기믹 베이스가 제네릭이므로 특정하기가 어려워서 만든 인터페이스 
/// </summary>
public interface IGimmickBase
{
    public void SetGimmick();
}

public abstract class GimmickBase<T> : MonoBehaviour, IGimmickBase where T : GimmickDataBase
{
    // [Header("Gimmick Base")]
    // /// <summary> 기믹과 상호작용하는 타겟 </summary>
    // [SerializeField] protected GimmickDefines.TargetType targetType;
    // /// <summary> 기믹 활성화 여부 </summary>
    // [SerializeField] protected bool isActive = true;
    //
    // /// <summary> 기믹과 상호작용하는 타겟 </summary>
    // public GimmickDefines.TargetType TargetType => targetType;

    /// <summary>
    /// 기믹에서 사용되는 데이터
    /// 반드시 가지고 있어야 하며, 변수의 이름은 gimmickData에서 변경되면 안된다.
    /// </summary>
    [SerializeField] protected T gimmickData;
    [SerializeField] protected string prefabName;

    public T GimmickData => gimmickData;

    /// <summary> 모든 기믹 공통 동작: 활성화/비활성화 </summary>
    // protected virtual void Activate()
    // {
    //     if (isActive)
    //     {
    //         // 기믹이 실행될 때 동작
    //         Interact();
    //     }
    // }
    
    private void Awake()
    {
        gimmickData.Init(transform, GetAddress());
        Init();
    }

    /// <summary>
    /// 객체가 생성되고 나서 1회 호출되는 초기화 함수
    /// </summary>
    protected abstract void Init();

    /// <summary>
    /// 기믹을 현재 gimmickData에 있는 값을 기준으로 세팅한다.
    /// </summary>
    public abstract void SetGimmick();

    /// <summary>
    /// 어드레스를 반환하는 함수. 반드시 구현이 되어야 함.
    /// </summary>
    protected virtual string GetAddress()
    {
        return $"Data/Prefabs/Gimmick/{prefabName}";
    }

    /// <summary> 기믹의 실제 동작은 각 자식 클래스에서 정의 </summary>
    // protected abstract void Interact();
}
