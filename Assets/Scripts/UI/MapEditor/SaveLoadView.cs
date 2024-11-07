using System;
using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadView : MonoBehaviour
{
    [SerializeField] InputField inputFileName;
    [SerializeField] Button btnSave;
    [SerializeField] Button btnLoad;

    public string FileName => inputFileName.text;

    private void Start()
    {
        UIHelper.OnClick(btnSave, OnClickSave);
        UIHelper.OnClick(btnLoad, OnClickLoad);
    }

    private void OnClickSave()
    {
        if (inputFileName.text == string.Empty) return;

        var gimmickDataList = GridData.Instance.GetGimmickDataBaseList();
        StageManager.Instance.SaveStage(FileName, gimmickDataList);
    }

    private void OnClickLoad()
    {
        if (inputFileName.text == string.Empty) return;

        if (SerializeManager.Instance.IsFileExist(FileName) == false) return;

        var mapData = StageManager.Instance.LoadStage(FileName);
    }
}
