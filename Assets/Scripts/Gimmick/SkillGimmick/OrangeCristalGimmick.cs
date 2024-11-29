using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeCristalGimmick : GimmickBase<CristalData>
{
    [SerializeField] private Cristal cristal;
    [SerializeField] private CristalSense sense;
    [SerializeField] private Transform plate;

    private string PlatePosition = "PlatePosition";

    private bool isCristalBroke = false;

    protected override void Init()
    {
        cristal.Init();

        sense.OnCristalBreak.RemoveListener(OnCristalBreak);
        sense.OnCristalBreak.AddListener(OnCristalBreak);
    }

    [ContextMenu("SetMenu")]
    public override void SetGimmick()
    {
        cristal.SetGimmick();

        isCristalBroke = false;

        plate.position = gimmickData.DictPoint[PlatePosition].position;
        plate.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(isCristalBroke == false)
        {
            cristal.cristalUpdate?.Invoke();
        }
    }

    public void OnCristalBreak()
    {
        isCristalBroke = true;
        cristal.OnCristalBreak();
        ActivateGimmick();
    }

    public void ActivateGimmick()
    {
        plate.gameObject.SetActive(true);
    }
}
