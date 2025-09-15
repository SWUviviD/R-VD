using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearPoint : GimmickBase<StageClearPointData>
{
    [SerializeField] private Transform clearArea;
    [SerializeField] private Collider[] stageBoundarys;

    protected override void Init()
    {
        SetGimmick();
    }

    public override void SetGimmick()
    {
        foreach (Collider c in stageBoundarys)
        {
            c.isTrigger = true;
        }

        //clearArea.localScale = gimmickData.AreaScale;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerHp>(out var hp) == true)
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

    public void GameClearedAndMoveToNextStage(bool isMainCam)
    {
        CameraController.Instance.GetCameraEffector?.
            FadeInOut(false, 2f, () => GameManager.Instance.NextStage());
    }
}
