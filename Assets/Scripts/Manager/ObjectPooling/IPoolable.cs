using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines.PoolDefines;

public interface IPoolable
{
    public delegate void ReturnToPool(PoolType type, IPoolable obj);
    protected ReturnToPool returnToPool { get; set; }
    IPoolable Create(ReturnToPool returnToPool);
    void Enqueue();
    void Dequeue();
}
