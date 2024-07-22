using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines.PoolDefines;
using UnityEngine.Pool;

public class PoolManager : MonoSingleton<PoolManager>
{
    private Dictionary<PoolType, IPoolable> poolOrigin;
    private Dictionary<PoolType, Queue<IPoolable>> pools;

    protected override void Init()
    {
        poolOrigin = new Dictionary<PoolType, IPoolable>();
        pools = new Dictionary<PoolType, Queue<IPoolable>>();
    }

    public bool CreatePool(PoolType poolType, IPoolable poolObject, int capacity = 10)
    {
        // 이미 풀이 있음
        if (poolOrigin.ContainsKey(poolType))
            return true;

        // 풀 만들기
        poolOrigin[poolType] = poolObject;
        pools[poolType] = new Queue<IPoolable>();
        for(int i = 0; i < capacity; i++)
        {
            EnqueuePoolObject(poolType);
        }

        return true;
    }

    public IPoolable GetPoolObject(PoolType poolType)
    {
        if (pools.ContainsKey(poolType) == false)
            return null;

        IPoolable clone;
        if(pools[poolType].TryDequeue(out clone) == false)
        {
            EnqueuePoolObject(poolType);
            clone = pools[poolType].Dequeue();
        }
        clone.Dequeue();

        return clone;
    }

    private void EnqueuePoolObject(PoolType poolType)
    {
        IPoolable clone = poolOrigin[poolType].Create(ReturnToPool);
        pools[poolType].Enqueue(clone);
    }

    private void ReturnToPool(PoolType type, IPoolable obj)
    {
        obj.Enqueue();
        pools[type].Enqueue(obj);
    }
}
