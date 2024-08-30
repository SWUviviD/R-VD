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

    [Header("Toast Popup")]
    [SerializeField] private Button btnToastPopup;
    [SerializeField] private InputField contentInput;

    public void Set(bool showClose)
    {
        goClose.SetActive(showClose);
    }
    
    private void Start()
    {
        btnShowWindow.onClick.AddListener(OnClickShowWindow);
        btnToastPopup.onClick.AddListener(OnClickToastPopup);
    }

    private void OnClickShowWindow()
    {
        UIManager.Instance.Show(UIDefines.UISampleWindow, null);
    }

    private void OnClickToastPopup()
    {
        UIManager.Instance.ToastPopup(contentInput.text);
    }
}
