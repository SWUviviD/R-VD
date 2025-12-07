using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] protected Renderer render;

    [SerializeField] protected UnityEvent OnActivated = new UnityEvent();

    protected void Start()
    {
        currentState = State.Stopped;
        render.material.color = Color.white;
    }

    public override void OnShocked(ShockableObj obj)
    {
        if (currentState != State.Stopped) return;

        currentState = State.Generating;

        // Play Animation & Sound
        // Temp Effect
        StartCoroutine(CoGenerating());
    }

    protected virtual IEnumerator CoGenerating()
    {
        // temp effect
        yield return new WaitForSeconds(0.5f);

        render.material.color = Color.blue;
        foreach(var shockObj in shockObj)
        {
            shockObj?.OnShocked(this);
        }
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        if (currentState != State.Generating) return;

        currentState = State.Stopped;

        // temp effect
        StartCoroutine(CoStopped());
    }

    protected virtual IEnumerator CoStopped()
    {
        // temp effect
        yield return new WaitForSeconds(0.2f);

        render.material.color = Color.white;
        foreach (var shockObj in shockObj)
        {
            shockObj?.ShockFailed(this);
        }
    }

    protected void StopGenerating()
    {
        ShockFailed();
    }
}
