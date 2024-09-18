using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GimmickCollision : GimmickBase
{
    /// <summary> 충돌 기반 상호작용 </summary>
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(TargetType.ToString()))
        {
            Activate();
        }
    }
}
