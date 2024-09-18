using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    [Header("Gimmick Base")]
    /// <summary> 기믹과 상호작용하는 타겟 </summary>
    [SerializeField] protected GimmickDefines.TargetType targetType;
    /// <summary> 기믹 활성화 여부 </summary>
    [SerializeField] protected bool isActive = true;

    /// <summary> 기믹과 상호작용하는 타겟 </summary>
    public GimmickDefines.TargetType TargetType => targetType;

    /// <summary> 모든 기믹 공통 동작: 활성화/비활성화 </summary>
    protected virtual void Activate()
    {
        if (isActive)
        {
            // 기믹이 실행될 때 동작
            Interact();
        }
    }

    /// <summary> 기믹의 실제 동작은 각 자식 클래스에서 정의 </summary>
    protected abstract void Interact();

    /// <summary> 디버깅 버튼 (TODO: 삭제) </summary>
    public void OnInteraction()
    {
        Interact();
    }
}
