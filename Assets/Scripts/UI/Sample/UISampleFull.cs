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
    [SerializeField] private InputField titleInput;
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

    // 토스트 팝업 생성
    private void OnClickToastPopup()
    {
        UIManager.Instance.Show(UIDefines.UISampleToastPopup, (ui) => ToastPopupAction(ui));
    }

    // 토스트 팝업 내용 수정 대리자 (Title, Content)
    private void ToastPopupAction(UIBase ui)
    {
        if (ui.TryGetComponent(out UISampleToastPopup toastPopup))
        {
            toastPopup.SetToastPopupText(titleInput.text, contentInput.text);
        }
    }
}
