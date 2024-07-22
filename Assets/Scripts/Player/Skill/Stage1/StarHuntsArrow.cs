using Defines.PoolDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StarHuntsArrow : MonoBehaviour, IPoolable
{
    IPoolable.ReturnToPool _returnToPool;

    IPoolable.ReturnToPool IPoolable.returnToPool 
    { 
        get => _returnToPool; 
        set => _returnToPool = value; 
    }

    IPoolable IPoolable.Create(IPoolable.ReturnToPool returnToPool)
    {
        GameObject arrow = (AddressableAssetsManager.Instance.SyncLoadObject(
            AddressableAssetsManager.Instance.GetPrefabPath("Stage1/", "StarHuntsArrow.prefab"),
            PoolType.StarHunts.ToString())) as GameObject;
        if (arrow == null)
            return null;

        IPoolable clone = Instantiate(arrow).GetComponent<IPoolable>();
        this._returnToPool = returnToPool;
        return clone;
    }

    void IPoolable.Dequeue()
    {
        gameObject.SetActive(true);
    }

    void IPoolable.Enqueue()
    {
        gameObject.SetActive(false);
        _returnToPool.Invoke(PoolType.StarHunts, this);
    }
}
