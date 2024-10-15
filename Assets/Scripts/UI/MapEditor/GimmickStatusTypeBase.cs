using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGimmickStatusTypeBase { }

/// <summary>
/// 기믹 스테이터스에 보이는 리스트 라인 베이스
/// </summary>
public abstract class GimmickStatusTypeBase<T> : MonoBehaviour, IGimmickStatusTypeBase
{

    protected object targetObject;
    protected System.Action<object, object> setProperty;
    
    /// <summary>
    /// 이름과 값을 설정한다.
    /// 프로퍼티의 값을 세팅할 수 있는 함수도 설정한다.
    /// </summary>
    public abstract void Set(string _name, T _value, object _targetObject, System.Action<object, object> _setProperty);
}
