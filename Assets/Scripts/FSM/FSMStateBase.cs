using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;

public abstract class FSMStateBase
{
    public FSMDefines.FSMState MyState { get; set; }

    public virtual void Init() { }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();

    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdaate() { }
    public virtual void OnLateUpdate() { }


}
