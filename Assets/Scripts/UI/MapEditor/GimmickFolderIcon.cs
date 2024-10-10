using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 기믹뷰에서 기믹이나 폴더를 표현하는 아이콘.
/// 상황에 따라서 기믹을 표현하기도 하고, 폴더를 표현하기도 한다.
/// </summary>
public class GimmickFolderIcon : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtName;
    [SerializeField] private Button btnSelect;
    [SerializeField] private GameObject objSelected;

    [SerializeField] private Sprite sprFolder;
    [SerializeField] private Sprite sprGimmick;

    public bool IsGimmick { get; private set; } 
    public string IconName { get; private set; }
    /// <summary> 실제 파일이 위치하는 경로 </summary>
    public string Path { get; private set; }
    /// <summary> 어드레서블 상에서의 경로 </summary>
    public string AddressPath { get; private set; }
    public bool IsSelected => objSelected.activeSelf;

    public void Init(Action<GimmickFolderIcon> _onClick)
    {
        btnSelect.onClick.AddListener(() => _onClick.Invoke(this));
        objSelected.SetActive(false);
    }
    
    public void Set(bool _isGimmick, string _iconName, string _path)
    {
        objSelected.SetActive(false);
        IconName = _iconName;
        IsGimmick = _isGimmick;
        Path = _path;
        AddressPath = GetAddressablePath(_path);
        txtName.text = _iconName;

        imgIcon.sprite = _isGimmick ? sprGimmick : sprFolder;
    }

    public void SetSelect(bool _set)
    {
        objSelected.SetActive(_set);
    }
    
    private string GetAddressablePath(string _path)
    {
        var relativePath = System.IO.Path.GetRelativePath(GimmickFolderView.GimmickFolderAbsolutePath, _path);
        return relativePath;
    }
}
