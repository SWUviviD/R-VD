using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 기믹이 보관된 폴더에 존재하는 모든 기믹들과 폴더 정보를 가져온다.
/// </summary>
public class GimmickFolderView : MonoBehaviour
{
    private const string GimmickFolderPath = "Data/Prefabs/Gimmick";

    private DirectoryInfo gimmickDirectoryInfo;

    [SerializeField] private GimmickFolderIcon gimmickIconPrefab;

    /// <summary> 뷰를 열고 닫는 토글버튼 </summary>
    [SerializeField] private Button btnToggle;
    /// <summary> 토글 이미지 </summary>
    [SerializeField] private Image imgToggle;

    /// <summary> 뷰를 여는 토글 이미지 </summary>
    [SerializeField] private Sprite sprOpenToggle;
    /// <summary> 뷰를 닫는 토글 이미지 </summary>
    [SerializeField] private Sprite sprCloseToggle;

    [SerializeField] private Transform trContent;
    
    /// <summary> 기믹을 선택한 경우 호출되는 콜백 </summary>
    public Action OnSelectGimmick { get; set; }

    /// <summary> 뷰가 열려있는지 여부 </summary>
    private bool isOpen;

    /// <summary> 뷰의 너비. 뷰를 열고 닫을때 너비를 기준으로 동작한다. </summary>
    private float viewWidth;

    private List<GimmickFolderIcon> gimmickIconList;
    
    private void Start()
    {
        UIHelper.OnClick(btnToggle, OnClickToggle);

        gimmickIconList = new List<GimmickFolderIcon>();
        gimmickDirectoryInfo = new DirectoryInfo(Path.Combine(Application.dataPath, GimmickFolderPath));
        
        var rtTransform = transform as RectTransform;
        viewWidth = rtTransform.sizeDelta.x;
        isOpen = false;
        
        Refresh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnClickToggle();
        }
    }

    /// <summary>
    /// 화면을 갱신한다.
    /// </summary>
    private void Refresh()
    {
        imgToggle.sprite = isOpen ? sprOpenToggle : sprCloseToggle;

        var rtView = transform as RectTransform;
        rtView.pivot = new Vector2(isOpen ? 1f: 0f, 0.5f);
        rtView.anchoredPosition = Vector2.zero;

        if (isOpen)
        {
            var files = gimmickDirectoryInfo.GetFiles("*.prefab");
            var folders = gimmickDirectoryInfo.GetDirectories();
            var totalCount = files.Length + folders.Length;

            if (gimmickIconList.Count < totalCount)
            {
                while (gimmickIconList.Count < totalCount)
                {
                    gimmickIconList.Add(Instantiate(gimmickIconPrefab, trContent));
                }
            }
            
            for (int i = 0, count = gimmickIconList.Count; i < count; ++i)
            {
                bool isActive = i < totalCount;
                gimmickIconList[i].gameObject.SetActive(isActive);
                if (isActive)
                {
                    bool isGimmick = i >= folders.Length;
                    string name = isGimmick ? files[i - folders.Length].Name : folders[i].Name;
                    string path = isGimmick ? files[i - folders.Length].FullName : folders[i].FullName;
                    gimmickIconList[i].Init(isGimmick, name, path);
                }
            }
        }
        else
        {
            gimmickIconList.ForEach(_ => _.gameObject.SetActive(false));
        }
    }

    private void OnClickToggle()
    {
        isOpen = !isOpen;
        
        Refresh();
    }
}
