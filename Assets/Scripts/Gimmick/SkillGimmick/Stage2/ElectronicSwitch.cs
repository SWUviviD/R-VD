using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitch : ShockableObj
{
    protected enum State
    {
        Generating,
        Stopped,
        Max
    }

    protected State currentState = State.Stopped;
    [SerializeField] protected ShockableObj[] shockObj;

    // temp Effect
    [SerializeField] private Renderer render;

    protected void Start()
    {
        currentState = State.Stopped;
        render.material.color = Color.white;
    }

    public override void OnShocked(ShockableObj _obj)
    {
        if (currentState != State.Stopped) return;

        preShockedObj = _obj;
        currentState = State.Generating;

        // Play Animation & Sound
        // Temp Effect
        StartCoroutine(CoGenerating());
    }

    private IEnumerator CoGenerating()
    {
        // temp effect
        yield return new WaitForSeconds(0.5f);

        render.material.color = Color.blue;
        foreach(var shockObj in shockObj)
        {
            shockObj?.OnShocked(this);
        }
    }

    public override void ShockFailed()
    {
        if (currentState != State.Generating) return;

        currentState = State.Stopped;

        // temp effect
        StartCoroutine(CoStopped());
    }

    private IEnumerator CoStopped()
    {
        // temp effect
        yield return new WaitForSeconds(0.2f);

        render.material.color = Color.white;
        //shockObj?.ShockFailed();
    }
}
