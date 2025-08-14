using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicSwitchA : ElectronicSwitch, IFusionable
{
    private ElectronicMap map;
    private bool isConnectToMap = false;
    private int mapObjIndex = -1;

    public GameObject ellectricEffect;

    public override void SetForMap(ElectronicMap map, int index)
    {
        this.map = map;
        isConnectToMap = true;
        mapObjIndex = index;
    }

    [ContextMenu("Activate")]
    public bool Activate(Transform player)
    {
        if(currentState == State.Stopped)
            OnShocked(null);
        else if(currentState == State.Generating)
            StopGenerating();

        GameObject ellectric = Instantiate(ellectricEffect, transform.position, Quaternion.identity);
        Destroy(ellectric, 1f);

        return true;
    }

    protected override IEnumerator CoGenerating()
    {
        yield return new WaitForSeconds(0.2f);

        render.material.color = Color.blue;
        if (isConnectToMap)
        {
            map.ShockPin(mapObjIndex);
        }

        foreach (var obj in shockObj)
        {
            obj.OnShocked(this);
        }
    }

    protected override IEnumerator CoStopped()
    {
        yield return new WaitForSeconds(0.2f);

        render.material.color = Color.white;
        if (isConnectToMap)
        {
            map.ShockFail(mapObjIndex);
        }

        foreach (var item in shockObj)
        {
            item.ShockFailed();
            GameObject ellectric = Instantiate(ellectricEffect, transform.position, Quaternion.identity);
            Destroy(ellectric, 1f);
        }
    }
}
