using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StarHuntArrow : Poolable
{
    [SerializeField] private Rigidbody rigib;
    [SerializeField] private float speed = 1.0f;

    private Vector3 startPosition;
    private float range;

    public void Project(Transform _parent, float _range)
    {
        //transform.SetParent(_parent);

        range = _range;
        rigib.Move(_parent.position, Quaternion.identity);
        rigib.velocity = rigib.transform.forward * speed;
        startPosition = _parent.position;
    }

    private void FixedUpdate()
    {
        if ((rigib.transform.position - startPosition).magnitude >= range)
        {
            returnToPool.Invoke(Defines.PoolDefines.PoolType.StarHunts, this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 여기에 크리스탈 처리
    }
}

public partial class StarHuntArrow : Poolable
{ 
    public override Poolable Create(ReturnToPool _returnToPool)
    {
        GameObject platform = (AddressableAssetsManager.Instance.SyncLoadObject(
           AddressableAssetsManager.Instance.GetPrefabPath("Prefabs", "Arrow.prefab"),
           PoolDefines.PoolType.StarHunts.ToString())) as GameObject;
        if (platform == null)
            return null;

        Poolable clone = Instantiate(platform).GetComponent<Poolable>();
        clone.returnToPool = _returnToPool;
        return clone;
    }

    public override void Dequeue()
    {
        gameObject.SetActive(true);
    }

    public override void Enqueue()
    {
        gameObject.SetActive(false);
    }
}
