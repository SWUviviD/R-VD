using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicPin : ShockableObj, IFusionable
{
    public enum State
    {
        Inactive,
        Active,
        Stabled,
        Max
    }

    public enum Dir
    {
        Forward = 0,
        Backward = 1,
        Left = 2,
        Right = 3
    }

    [SerializeField] private ElectronicPin[] pins = new ElectronicPin[4];
    public State CurrentState;

    private Renderer render;

    public bool Activate(Transform player)
    {
        switch(CurrentState)
        {
            case State.Inactive:
                {
                    StartCoroutine(CoActivate());
                    break;
                }
            case State.Active:
                {
                    StartCoroutine(CoInactivate());
                    break;
                }
        }

        return true;
    }

    private IEnumerator CoActivate()
    {
        yield return null;
        render.material.color = Color.green;
    }

    private IEnumerator CoInactivate()
    {
        yield return null;
        render.material.color = Color.gray;
    }

    private IEnumerator CoStable()
    {
        yield return null;
        render.material.color = Color.blue;
    }

    public override void OnShocked(ShockableObj _obj)
    {
        preShockedObj = _obj;

        foreach (var pin in pins)
        {


        }
    }

    public override void ShockFailed()
    {
    }
}
