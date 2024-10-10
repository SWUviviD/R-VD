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
    public static string GimmickFolderAbsolutePath = Path.Combine(Application.dataPath, GimmickFolderPath);
    private const string GimmickFolderPath = "Data/Prefabs/Gimmick";

    private const string ParentFolderName = "../";
    
    /// <summary> 최상위 기믹 디렉토리 </summary>
    private DirectoryInfo gimmickDirectoryInfo;
    /// <summary> 현재 기믹 디렉토리 </summary>
    private DirectoryInfo currentDirectoryInfo;

    [SerializeField] private GimmickFolderIcon gimmickIconPrefab;

    /// <summary> 뷰를 열고 닫는 토글버튼 </summary>
    [SerializeField] private Button btnToggle;
    /// <summary> 토글 이미지 </summary>
    [SerializeField] private Image imgToggle;
    [SerializeField] private Button btnViewport;

    /// <summary> 뷰를 여는 토글 이미지 </summary>
    [SerializeField] private Sprite sprOpenToggle;
    /// <summary> 뷰를 닫는 토글 이미지 </summary>
    [SerializeField] private Sprite sprCloseToggle;
    [SerializeField] private Transform trContent;
    
    /// <summary> 기믹을 선택한 경우 호출되는 콜백 </summary>
    public Action<GimmickFolderIcon> OnSelectGimmick { get; set; }

    /// <summary> 뷰가 열려있는지 여부 </summary>
    private bool isOpen;

    /// <summary> 뷰의 너비. 뷰를 열고 닫을때 너비를 기준으로 동작한다. </summary>
    private float viewWidth;

    private List<GimmickFolderIcon> gimmickIconList;
    /// <summary> 현재 선택된 기믹 아이콘. </summary>
    private GimmickFolderIcon selectedGimmickIcon;
    
    private void Start()
    {
        UIHelper.OnClick(btnToggle, OnClickToggle);
        btnViewport.onClick.AddListener(() => OnClickIcon(null));

        gimmickIconList = new List<GimmickFolderIcon>();
        gimmickDirectoryInfo = new DirectoryInfo(GimmickFolderAbsolutePath);
        currentDirectoryInfo = gimmickDirectoryInfo;
        
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
        OnClickIcon(null);
        imgToggle.sprite = isOpen ? sprOpenToggle : sprCloseToggle;

        var rtView = transform as RectTransform;
        rtView.pivot = new Vector2(isOpen ? 1f: 0f, 0.5f);
        rtView.anchoredPosition = Vector2.zero;

        if (isOpen)
        {
            var files = currentDirectoryInfo.GetFiles("*.prefab");
            var folders = currentDirectoryInfo.GetDirectories();
            var totalCount = files.Length + folders.Length;

            // 기믹 아이콘이 totalCount보다 수가 적다면 추가로 생성한다.
            // 이때 여유분으로 1개를 추가로 더 만든다.(부모로 가는 폴더 생성을 위함)
            if (gimmickIconList.Count < totalCount + 1)
            {
                while (gimmickIconList.Count < totalCount + 1)
                {
                    var gimmickInstance = Instantiate(gimmickIconPrefab, trContent);
                    gimmickInstance.Init(OnClickIcon);
                    gimmickIconList.Add(gimmickInstance);
                }
            }
            
            for (int i = 0, count = gimmickIconList.Count; i < count; ++i)
            {
                bool isActive = i < totalCount;
                gimmickIconList[i].gameObject.SetActive(isActive);
                if (isActive)
                {
                    bool isGimmick = i >= folders.Length;
                    string iconName = isGimmick ? files[i - folders.Length].Name.Split(".")[0] : folders[i].Name;
                    string path = isGimmick ? files[i - folders.Length].FullName : folders[i].FullName;
                    gimmickIconList[i].Set(isGimmick, iconName, path);
                    gimmickIconList[i].transform.SetSiblingIndex(i);
                }
            }
            
            // 현재 폴더가 최상위 루트가 아니면, 뒤로가기 폴더가 필요하다.
            if (currentDirectoryInfo.FullName != gimmickDirectoryInfo.FullName)
            {
                // 부모로 이동하기 위한 폴더
                var parentFolder = gimmickIconList[totalCount]; 
                parentFolder.gameObject.SetActive(true);
                parentFolder.Set(false, ParentFolderName, string.Empty);
                // transform 위치를 0으로 옮겨서 맨 상단 좌측엔 무조건 부모로 이동하는 폴더를 위치시킨다.
                parentFolder.transform.SetSiblingIndex(0);
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

    private void OnClickIcon(GimmickFolderIcon _icon)
    {
        // 아이콘이 비어있으면, 기존에 선택한 아이콘은 선택 해제한다.
        if (_icon == null)
        {
            selectedGimmickIcon?.SetSelect(false);
            selectedGimmickIcon = null;
            return;
        }
        
        // 새로운 아이콘을 선택했다면 기존 아이콘의 선택을 해제하고 새로운 아이콘을 선택한다.
        if (_icon.IsSelected == false)
        {
            selectedGimmickIcon?.SetSelect(false);
            selectedGimmickIcon = _icon;
            selectedGimmickIcon.SetSelect(true);
            return;
        }
        
        _icon.SetSelect(false);
        
        if (_icon.IsGimmick)
        {
            OnSelectGimmick?.Invoke(_icon);
        }
        else
        {
            if (_icon.IconName == ParentFolderName)
            {
                // 부모로 이동한다.
                currentDirectoryInfo = currentDirectoryInfo.Parent;
            }
            else
            {
                // 해당 폴더로 이동한다.;
                currentDirectoryInfo = new DirectoryInfo(Path.Combine(currentDirectoryInfo.FullName, _icon.IconName));
            }
            Refresh();
        }
    }
}
