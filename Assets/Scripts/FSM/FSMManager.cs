using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines.FSMDefines;

public class FSMManager : MonoBehaviour
{
    private FSMData fsmData;
    private Dictionary<FSMState, FSMStateBase> stateDic;

    public FSMStateBase CurrentState { get; private set; }
    public FSMStateBase PrevState { get; private set; }

    private bool isThereState = false;

    private void Awake()
    {
        isThereState = false;
    }

    public bool ChangeState(FSMState state)
    {
        if(isThereState)
        {
            CurrentState.OnStateExit();
            PrevState = CurrentState;
        }

        if (stateDic.ContainsKey(state))
        {
            CurrentState = stateDic[state];
            CurrentState.OnStateEnter();
            isThereState = true;
        }

        return isThereState;
    }

    public void FixedUpdate()
    {
        if(isThereState)
        {
            CurrentState.OnFixedUpdate();
        }
    }

    public void Update()
    {
        if (isThereState)
        {
            CurrentState.OnUpdaate();
        }
    }

    public void LateUpdate()
    {
        if (isThereState)
        {
            CurrentState.OnLateUpdate();
        }
    }
}
