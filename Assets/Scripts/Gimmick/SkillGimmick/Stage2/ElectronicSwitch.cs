using System.Collections;
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

    [SerializeField] protected UnityEvent OnActivated = new UnityEvent();

    protected virtual void Start()
    {
        currentState = State.Stopped;
    }

    public override void OnShocked(ShockableObj obj)
    {
        if (currentState == State.Generating) return;

        currentState = State.Generating;

        // Play Animation & Sound
        // Temp Effect
        StartCoroutine(CoGenerating());
    }

    protected virtual IEnumerator CoGenerating()
    {
        // temp effect
        yield return new WaitForSeconds(0.5f);

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
