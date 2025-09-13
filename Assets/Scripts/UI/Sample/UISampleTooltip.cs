using Defines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UISampleTooltip : UIClamp
{
    [Header("UI Tooltip")]
    [SerializeField] private Text tooltipText;

    /// <summary> 나타낼 툴팁 문구 입력 </summary>
    public void SetContent(string content)
    {
        tooltipText.text = content;
    }
}

public partial class UISampleTooltip : UIClamp
{
    [Header("Pool Type")]
    [SerializeField] private PoolDefines.PoolType type;

    public override Poolable Create(ReturnToPool returnToPool)
    {
        GameObject tooltip = AssetLoadManager.Instance.SyncLoadObject(
            AssetLoadManager.Instance.GetPrefabPath("UI/Sample", "UISampleTooltip.prefab"),
            PoolDefines.PoolType.UITooltip.ToString()) as GameObject;
        if (tooltip == null)
            return null;

        Poolable clone = Instantiate(tooltip).GetComponent<Poolable>();
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