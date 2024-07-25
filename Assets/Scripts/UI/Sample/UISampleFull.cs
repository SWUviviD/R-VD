using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UnityEngine.UI;

public class UISampleFull : UIBase
{
    [SerializeField] private Button btnShowWindow;
    [SerializeField] private GameObject goClose;

    public void Set(bool showClose)
    {
        goClose.SetActive(showClose);
    }
    
    private void Start()
    {
        btnShowWindow.onClick.AddListener(OnClickShowWindow);
    }

    private void OnClickShowWindow()
    {
        UIManager.Instance.Show(UIDefines.UISampleWindow, null);
    }
}
