using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRail : ShockableObj
{
    [SerializeField] private Cart[] carts;
    public override void OnShocked(ShockableObj _obj)
    {
        foreach (var c in carts)
        {
            c.StartMoving();
        }
    }

    public override void ShockFailed() { }
}
