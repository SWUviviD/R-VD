using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetModelsByStage : MonoBehaviour
{
    private enum Stage
    {
        St1,
        St2,
        St3,
        MAX
    }

    [Serializable]
    private class StagelModels
    {
        [SerializeField]
        private GameObject[] models;

        public void SetModels(bool isOn)
        {
            foreach (var model in models)
            {
                model.SetActive(isOn);
            }
        }
    }

    [SerializeField] private Stage stage;
    [SerializeField] private StagelModels[] stagelModels = new StagelModels[(int) Stage.MAX];

    private void Awake()
    {
        int stageNum = (int) stage;
        int s = 0;
        foreach(var model in stagelModels)
        {
            model.SetModels(s == stageNum);
            ++s;
        }
    }
}
