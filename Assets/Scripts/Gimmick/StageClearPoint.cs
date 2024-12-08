using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearPoint : GimmickBase<StageClearPointData>
{
    [SerializeField] private Transform clearArea;
    [SerializeField] private Collider[] stageBoundarys;

    protected override void Init()
    {
    }

    public override void SetGimmick()
    {
        foreach (Collider c in stageBoundarys)
        {
            c.isTrigger = true;
        }

        clearArea.localScale = gimmickData.AreaScale;
    }

    private void OnTriggerExit(Collider other)
    {
        Transform parent = other.transform.parent;
        if (parent == null)
            return;

        if (parent.TryGetComponent<PlayerHp>(out var hp) == true)
        {
            OnGameClear();
        }
    }

    private void OnGameClear()
    {
        foreach(Collider c in stageBoundarys)
        {
            c.isTrigger = false;
        }

        GameManager.Instance.GameClear();
    }
}
