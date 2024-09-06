using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;

public abstract class Poolable : MonoBehaviour
{
    public delegate void ReturnToPool(PoolDefines.PoolType type, Poolable obj);
    protected ReturnToPool returnToPool { get; set; }
    public abstract Poolable Create(ReturnToPool returnToPool);
    public abstract void Enqueue();
    public abstract void Dequeue();
}
