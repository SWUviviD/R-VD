using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitchB : ElectronicSwitch
{
    [SerializeField] private int connectCount;

    private int currentUnShockedCount;

    public GameObject ellectricEffect;

    public void Awake()
    {
        currentUnShockedCount = connectCount;
    }

    public override void OnShocked(ShockableObj obj)
    {
        if (currentState != State.Stopped) return;

        GiveShockObj = obj;

        --currentUnShockedCount;
        Debug.Log(currentUnShockedCount);
        if (currentUnShockedCount > 0)
            return;

        currentUnShockedCount = 0;
        currentState = State.Generating;
        StartCoroutine(CoGenerating());
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        base.ShockFailed();

        GameObject ellectric = Instantiate(ellectricEffect, transform.position, Quaternion.identity);
        Destroy(ellectric, 1f);

        ++currentUnShockedCount;
        Debug.Log(currentUnShockedCount);
    }
}
