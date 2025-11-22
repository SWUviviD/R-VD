using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTester : MonoBehaviour
{
    [SerializeField] private int stageNum;

    private void Awake()
    {
        SkillSwap sSwap = GameObject.FindAnyObjectByType<SkillSwap>();

        for(int i = 0; i< 3; ++i)
        {
            if(i < stageNum)
                sSwap.UnlockSkill(i);
        }
    }
}
