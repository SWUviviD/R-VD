using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicPipe : ShockableObj
{
    [SerializeField] private float shockSpeed = 0.02f;

    [SerializeField] private ShockableObj[] attached;
    private bool isSending = false;

    public GameObject ellectricEffect;

    public override void OnShocked(ShockableObj obj)
    {
        if (isSending == true)
            return;

        GiveShockObj = obj;
        StartCoroutine(CoSendShock());
    }

    private IEnumerator CoSendShock()
    {
        isSending = true;
        
        yield return new WaitForSeconds(shockSpeed);

        foreach(var shock in attached)
        {
            if (shock == GiveShockObj)
                continue;
            shock.OnShocked(this);
        }

        isSending = false;
    }

    public override void ShockFailed(ShockableObj obj = null)
    {
        if(isSending == true)
        {
            StopAllCoroutines();
        }

        GameObject ellectric = Instantiate(ellectricEffect, transform.position, Quaternion.identity);
        Destroy(ellectric, 1f);

        GiveShockObj = null;
        StartCoroutine(CoSendFail(obj));
    }

    private IEnumerator CoSendFail(ShockableObj shockFailObj)
    {
        isSending = true;

        yield return new WaitForSeconds(shockSpeed);

        foreach (var shock in attached)
        {
            if(shock == shockFailObj)
                continue;
            shock.ShockFailed(this);
        }

        isSending = false;
    }
}
