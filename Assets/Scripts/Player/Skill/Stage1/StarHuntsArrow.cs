using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StarHuntsArrow : Poolable
{
    public override Poolable Create(Poolable.ReturnToPool returnToPool)
    {
        GameObject arrow = (AddressableAssetsManager.Instance.SyncLoadObject(
            AddressableAssetsManager.Instance.GetPrefabPath("Stage1/", "StarHuntsArrow.prefab"),
            PoolDefines.PoolType.StarHunts.ToString())) as GameObject;
        if (arrow == null)
            return null;

        Poolable clone = Instantiate(arrow).GetComponent<Poolable>();
        this.returnToPool = returnToPool;
        return clone;
    }

    public override void Dequeue()
    {
        gameObject.SetActive(true);
    }

    public override void Enqueue()
    {
        gameObject.SetActive(false);
        returnToPool.Invoke(PoolDefines.PoolType.StarHunts, this);
    }
}
