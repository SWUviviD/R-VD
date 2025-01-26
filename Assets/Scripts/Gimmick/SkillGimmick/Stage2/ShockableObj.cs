using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShockableObj : MonoBehaviour 
{
    protected ShockableObj preShockedObj;

    public abstract void OnShocked(ShockableObj _obj);

    public abstract void ShockFailed();
}
