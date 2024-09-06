using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public partial class  WaterMillPlatform : Poolable
{
    private Vector3 position;
    public Vector3 Position
    {
        get => position;
        set
        {
            position = value;
            RigidBody.position = value;
        }
    }
    private WaterMill parent;

    public void SetWaterMill(WaterMill _parent)
    {
        parent = _parent;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal != Vector3.down)
            return;

        collision.transform.parent = transform;
    }

    public void OnCollisionExit(Collision collision)
    {
        collision.transform.parent = null;
    }
}

public partial class WaterMillPlatform : Poolable
{
    [SerializeField] private Rigidbody rigidBody;
    public Rigidbody RigidBody => rigidBody;
    [SerializeField] private PoolDefines.PoolType type;
    public override Poolable Create(Poolable.ReturnToPool returnToPool)
    {
        GameObject platform = (AddressableAssetsManager.Instance.SyncLoadObject(
            AddressableAssetsManager.Instance.GetPrefabPath("Stage2/", "watermillPlatform.prefab"),
            PoolDefines.PoolType.WaterMillPlatform.ToString())) as GameObject;
        if (platform == null)
            return null;

        Poolable clone = Instantiate(platform).GetComponent<Poolable>();
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
        returnToPool.Invoke(type, this);
    }
}
