using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;

public class StarHunts : SkillBase
{
    const int arrowInitCount = 10;

    public override void OnInit()
    {
        GameObject arrowPrefab = (AddressableAssetsManager.Instance.SyncLoadObject(
            AddressableAssetsManager.Instance.GetPrefabPath("Stage1", "StarHuntsArrow.prefab"),
            PoolDefines.PoolType.WaterMillPlatform.ToString())) as GameObject;
        if (arrowPrefab == null)
            return;

        IPoolable arrow = arrowPrefab.GetComponent<IPoolable>();
        if (arrow != null)
        {
            return;
        }

        PoolManager.Instance.CreatePool(PoolDefines.PoolType.StarHunts, arrow, arrowInitCount);
    }

    public override bool UseSkill(Status status)
    {
        return true;
    }
}
