using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage3StageChecker : MonoBehaviour
{
    [SerializeField] private CheckpointGimmick leftClear;
    [SerializeField] private Collider leftCollider;
    [SerializeField] private UnityEvent LeftClearEvent;

    [SerializeField] private CheckpointGimmick rightClear;
    [SerializeField] private Collider rightCollider;
    [SerializeField] private UnityEvent RightClearEvent;

    [SerializeField] private CheckpointGimmick allClear;
    [SerializeField] private Collider allCollider;

    private bool isLeftClear;
    private bool isRightClear;

    private void Awake()
    {
        if(GameManager.Instance.GetFlag(1) == true)
        {
            LeftClearEvent?.Invoke();
        }

        if (GameManager.Instance.GetFlag(2) == true)
        {
            RightClearEvent?.Invoke();
        }
    }

    public void CheckClear(bool isLeft)
    {
        leftCollider.gameObject.SetActive(false);
        rightCollider.gameObject.SetActive(false);
        allCollider.gameObject.SetActive(false);

        int index = CheckpointGimmick.CurrentCheckpointIndex;
        if (index < 0 || CheckpointGimmick.CheckpointList.ContainsKey(index) == false)
            return;

        if (CheckpointGimmick.CheckpointList[index].checkPointID >
            allClear.checkPointID)
            return;

        isLeftClear = isLeft == true ? true : isLeftClear;
        isRightClear = isLeft == false ? isRightClear : true;


        GameManager.Instance.SetFlag((isLeft ? 1 : 2), true);

        if(isLeftClear && isRightClear)
        {
            allCollider.gameObject.SetActive(true);
            leftCollider.gameObject.SetActive(false);
            rightCollider.gameObject.SetActive(false);
        }
        else if(isLeftClear)
        {
            allCollider.gameObject.SetActive(false);
            leftCollider.gameObject.SetActive(true);
            rightCollider.gameObject.SetActive(false);
        }
        else
        {
            allCollider.gameObject.SetActive(false);
            leftCollider.gameObject.SetActive(false);
            rightCollider.gameObject.SetActive(true);
        }
    }
}
