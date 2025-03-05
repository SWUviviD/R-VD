using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using LevelEditor;
#endif
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

#if UNITY_EDITOR
        var gimmickDataList = GridData.Instance.GetGimmickDataBaseList();
        var cameraPath = CameraPathInsertSystem.Instance.GetCameraPath();
        StageManager.Instance.SaveStage(FileName, gimmickDataList, cameraPath);
#endif
    }

    private void OnClickLoad()
    {
        if (inputFileName.text == string.Empty) return;

#if UNITY_EDITOR
        MapLoadManager.Instance.LoadMapInEditor(FileName);
#endif

        //if (SerializeManager.Instance.IsFileExist(FileName) == false) return;

        //var mapData = StageManager.Instance.LoadStage(FileName);
    }
}
