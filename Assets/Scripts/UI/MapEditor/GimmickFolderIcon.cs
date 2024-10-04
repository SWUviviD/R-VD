using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 기믹뷰에서 기믹이나 폴더를 표현하는 아이콘.
/// 상황에 따라서 기믹을 표현하기도 하고, 폴더를 표현하기도 한다.
/// </summary>
public class GimmickFolderIcon : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtName;

    [SerializeField] private Sprite sprFolder;
    [SerializeField] private Sprite sprGimmick;

    private string path;

    public void Init(bool _isGimmick, string _name, string _path)
    {
        path = _path;
        txtName.text = _name;

        imgIcon.sprite = _isGimmick ? sprGimmick : sprFolder;
    }
}
