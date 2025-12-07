using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageClearPoint : GimmickBase<StageClearPointData>
{
    [SerializeField] private Transform clearArea;
    [SerializeField] private Collider[] stageBoundarys;
    [SerializeField] private UnityEvent onStageCleared = new UnityEvent();

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

        onStageCleared?.Invoke();
        GameManager.Instance.GameClear();
    }

    public void GameClearedAndMoveToNextStage(bool isMainCam)
    {
        CameraController.Instance.GetCameraEffector?.
            FadeInOut(false, 2f, () => GameManager.Instance.NextStage());
    }
}
