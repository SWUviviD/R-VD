using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGimmick : GimmickBase<ChasingGimmickData>
{
    [SerializeField] public ChasingStarProp[] starList;
    public int currentStarIndex;

    protected override void Init()
    {
        // 별똥별 관리 배열 초기화
        starList = new ChasingStarProp[gimmickData.TotalNum];

        if (starList.Length == 1)
        {
            starList[0] = Instantiate(gimmickData.starListPrefab[0]);
        }
        else if (starList.Length == 2)
        {
            starList[0] = Instantiate(gimmickData.starListPrefab[1]);
            starList[1] = Instantiate(gimmickData.starListPrefab[3]);
        }
        else if (starList.Length == 3)
        {
            starList[0] = Instantiate(gimmickData.starListPrefab[1]);
            starList[1] = Instantiate(gimmickData.starListPrefab[3]);
            starList[2] = Instantiate(gimmickData.starListPrefab[3]);
        }
        else if (starList.Length == 4)
        {
            starList[0] = Instantiate(gimmickData.starListPrefab[2]);
            starList[1] = Instantiate(gimmickData.starListPrefab[2]);
            starList[2] = Instantiate(gimmickData.starListPrefab[2]);
            starList[3] = Instantiate(gimmickData.starListPrefab[2]);
        }

        // 프리팹 비가시화
        for (int i = 0; i < starList.Length; i++)
        {
            starList[i].gameObject.SetActive(false);
        }
    }

    public override void SetGimmick()
    {
        currentStarIndex = 0;
    }

    private IEnumerator DropStars()
    {
        while (true)
        {
            starList[currentStarIndex].StartFalling(gimmickData.StarFallSpeed, gimmickData.PlayerDamage);
            yield return new WaitForSeconds(gimmickData.ResponeTime);
            currentStarIndex = (currentStarIndex + 1) % starList.Length;

            if (currentStarIndex == 0)
            {
                foreach (var star in starList) star.ResetStar();
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DropStars());
    }
}
