using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRail : ShockableObj
{
    [SerializeField] private Cart[] carts;
    public override void OnShocked(ShockableObj obj)
    {
        foreach (var c in carts)
        {
            c.StartMoving();
        }
    }

    public override void ShockFailed(ShockableObj obj = null) { } // 카트는 발전이 종료되어도 계속 이동한다.
}
