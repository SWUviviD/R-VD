using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;

public class StarHunts : SkillBase
{
    private const int arrowInitCount = 10;

    public override void OnInit()
    {
        GameObject arrowPrefab = (AddressableAssetsManager.Instance.SyncLoadObject(
            AddressableAssetsManager.Instance.GetPrefabPath("Prefabs/Temp/Stage1", "StarHuntsArrow.prefab"),
            PoolDefines.PoolType.WaterMillPlatform.ToString())) as GameObject;
        if (arrowPrefab == null)
            return;

        Poolable arrow = arrowPrefab.GetComponent<Poolable>();
        if (arrow != null)
        {
            return;
        }

        PoolManager.Instance.CreatePool(PoolDefines.PoolType.StarHunts, arrow, arrowInitCount);
    }

    public override void OnSkill(PlayerStatus _playerStatus)
    {
    }

    public override void OnSkillStart(PlayerStatus _playerStatus)
    {
    }

    public override void OnSkillStop(PlayerStatus _playerStatus)
    {
    }
}
