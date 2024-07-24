using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;
using UnityEngine.Pool;

public class PoolManager : MonoSingleton<PoolManager>
{
    private Dictionary<PoolDefines.PoolType, IPoolable> poolOrigin;
    private Dictionary<PoolDefines.PoolType, Queue<IPoolable>> pools;

    protected override void Init()
    {
        poolOrigin = new Dictionary<PoolDefines.PoolType, IPoolable>();
        pools = new Dictionary<PoolDefines.PoolType, Queue<IPoolable>>();
    }

    public bool CreatePool(PoolDefines.PoolType poolType, IPoolable poolObject, int capacity = 10)
    {
        // �̹� Ǯ�� ����
        if (poolOrigin.ContainsKey(poolType))
            return true;

        // Ǯ �����
        poolOrigin[poolType] = poolObject;
        pools[poolType] = new Queue<IPoolable>();
        for(int i = 0; i < capacity; i++)
        {
            EnqueuePoolObject(poolType);
        }

        return true;
    }

    public IPoolable GetPoolObject(PoolDefines.PoolType poolType)
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

    private void EnqueuePoolObject(PoolDefines.PoolType poolType)
    {
        IPoolable clone = poolOrigin[poolType].Create(ReturnToPool);
        pools[poolType].Enqueue(clone);
    }

    private void ReturnToPool(PoolDefines.PoolType type, IPoolable obj)
    {
        obj.Enqueue();
        pools[type].Enqueue(obj);
    }
}
