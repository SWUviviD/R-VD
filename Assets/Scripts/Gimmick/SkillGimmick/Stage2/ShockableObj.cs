using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShockableObj : MonoBehaviour 
{
    public ShockableObj PowerSourceObj { get; protected set; }

    public virtual void SetForMap(ElectronicMap map, int index) { }

    public abstract void OnShocked(ShockableObj obj);

    public abstract void ShockFailed(ShockableObj obj = null);
}
