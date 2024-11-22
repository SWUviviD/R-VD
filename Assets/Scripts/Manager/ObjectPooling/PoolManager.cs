using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;
using UnityEngine.Pool;

public class PoolManager : MonoSingleton<PoolManager>
{
    private Dictionary<PoolDefines.PoolType, Poolable> poolOrigin;
    private Dictionary<PoolDefines.PoolType, Queue<Poolable>> pools;

    protected override void Init()
    {
        poolOrigin = new Dictionary<PoolDefines.PoolType, Poolable>();
        pools = new Dictionary<PoolDefines.PoolType, Queue<Poolable>>();
    }

    public bool CreatePool(PoolDefines.PoolType poolType, Poolable poolObject, int capacity = 10)
    {
        // 이미 풀이 있음
        if (poolOrigin.ContainsKey(poolType))
            return true;

        // 풀 만들기
        poolOrigin[poolType] = poolObject;
        pools[poolType] = new Queue<Poolable>();
        for(int i = 0; i < capacity; i++)
        {
            EnqueuePoolObject(poolType);
        }

        return true;
    }

    public Poolable GetPoolObject(PoolDefines.PoolType poolType)
    {
        if (pools.ContainsKey(poolType) == false)
            return null;

        Poolable clone;
        if(pools[poolType].TryDequeue(out clone) == false)
        {
            EnqueuePoolObject(poolType);
            clone = pools[poolType].Dequeue();
        }
        clone.Dequeue();

        return clone;
    }

    private void EnqueuePoolObject(PoolDefines.PoolType poolType)
    {
        Poolable clone = poolOrigin[poolType].Create(ReturnToPool);
        clone.gameObject.SetActive(false);
        pools[poolType].Enqueue(clone);
        clone.name += pools[poolType].Count.ToString();
    }

    private void ReturnToPool(PoolDefines.PoolType type, Poolable obj)
    {
        obj.Enqueue();
        pools[type].Enqueue(obj);
    }
}
