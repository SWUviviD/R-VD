using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3StageChecker : MonoBehaviour
{
    [SerializeField] private CheckpointGimmick leftClear;
    [SerializeField] private CheckpointGimmick rightClear;
    [SerializeField] private CheckpointGimmick allClear;

    private bool isLeftClear;
    private bool isRightClear;

    public void CheckClear(bool isLeft)
    {
        leftClear.gameObject.SetActive(false);
        rightClear.gameObject.SetActive(false);
        allClear.gameObject.SetActive(false);

        int index = CheckpointGimmick.CurrentCheckpointIndex;
        if (index < 0 || CheckpointGimmick.CheckpointList.ContainsKey(index) == false)
            return;

        if (CheckpointGimmick.CheckpointList[index].checkPointNumber >
            allClear.checkPointNumber)
            return;

        isLeftClear = isLeft == true;
        isRightClear = isLeft == false;

        if(isLeftClear && isRightClear)
        {
            allClear.gameObject.SetActive(true);
        }
        else if(isLeftClear)
        {
            leftClear.gameObject.SetActive(true);
        }
        else
        {
            rightClear.gameObject.SetActive(true);
        }
    }
}
