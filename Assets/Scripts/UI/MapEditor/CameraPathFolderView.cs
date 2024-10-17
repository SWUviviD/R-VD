using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CameraPathFolderView : MonoBehaviour
{
    [SerializeField] private Button btnToggle;
    /// <summary> 토글 이미지 </summary>
    [SerializeField] private Image imgToggle;
    /// <summary> 뷰를 여는 토글 이미지 </summary>
    [SerializeField] private Sprite sprOpenToggle;
    /// <summary> 뷰를 닫는 토글 이미지 </summary>
    [SerializeField] private Sprite sprCloseToggle;

    [SerializeField] private Button btnAddPoint;
    [SerializeField] private Button btnInsertPoint;

    [SerializeField] private CameraPathInsertSystem insertSystem;
    [SerializeField] private CameraPathInputSystem inputSystem;
    
    private bool isOpen;

    private void Start()
    {
        UIHelper.OnClick(btnToggle, OnClickToggle);
        UIHelper.OnClick(btnAddPoint, OnClickAddPoint);
        UIHelper.OnClick(btnInsertPoint, OnClickInsertPoint);
        
        isOpen = false;
        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnClickToggle();
        }
    }

    private void Refresh()
    {
        imgToggle.sprite = isOpen ? sprOpenToggle : sprCloseToggle;
        
        var rtView = transform as RectTransform;
        rtView.pivot = new Vector2(isOpen ? 1f: 0f, 0.5f);
        rtView.anchoredPosition = Vector2.zero;
    }
    
    private void OnClickToggle()
    {
        isOpen = !isOpen;
        
        Refresh();
    }

    private void OnClickAddPoint()
    {
        insertSystem.SetInsertMode(GimmickDefines.CameraPathInsertMode.Add);
    }

    private void OnClickInsertPoint()
    {
        insertSystem.SetInsertMode(GimmickDefines.CameraPathInsertMode.Insert);
    }
}
