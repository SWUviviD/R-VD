using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGimmick : GimmickBase<ChasingGimmickData>
{
    [SerializeField] private ChasingStarProp prefabStarProp;
    [SerializeField] private Transform panel;

    private List<ChasingStarProp> starList;
    private int currentStarIndex;

    protected override void Init()
    {
        for (int i = 0; i < starList.Count; i++)
        {
            starList.Add(Instantiate(prefabStarProp, panel));
            starList[i].gameObject.SetActive(false);
        }
    }

    public override void SetGimmick()
    {
        starList = new List<ChasingStarProp>();
        currentStarIndex = 0;
    }

    private IEnumerator DropStars()
    {
        while (true)
        {
            starList[currentStarIndex].StartFalling(gimmickData.StarFallSpeed, gimmickData.PlayerDamage);
            yield return new WaitForSeconds(gimmickData.StarShowInterval);
            currentStarIndex = (currentStarIndex + 1) % starList.Count;

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
