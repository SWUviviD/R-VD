using System;
using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class GimmickQuickSlot : MonoBehaviour
{
    private const int QuickSlotMaxCount = 10;

    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private GimmickFolderView folderView;
    
    /// <summary>
    /// 퀵슬롯 버튼 리스트. 1부터 시작해서 9까지 배치되고 마지막은 0이다.
    /// </summary>
    [SerializeField] private List<GimmickQuickSlotIcon> btnQuickSlotList;

    /// <summary> 슬롯에 대한 정보. 현재 슬롯의 정보로는 선택한 프리팹의 경로가 정의되어있다. </summary>
    private List<string> slotData;
    /// <summary> 현재 선택된 슬롯 </summary>
    private GimmickQuickSlotIcon currentIcon;
    /// <summary> 현재 선택된 아이콘 인덱스 </summary>
    private int currentIconIndex = 0;
    
    private void Start()
    {
        slotData = new List<string>(new string[QuickSlotMaxCount]);
        for(int i = 0; i < btnQuickSlotList.Count; ++i)
        {
            var btn = btnQuickSlotList[i];
            btn.Init(OnClickSlotIcon, i);
            btn.Select(false);
            btn.Set(null);
        }

        currentIcon = null;
        currentIconIndex = 1;
        SelectIcon(btnQuickSlotList[currentIconIndex]);

        folderView.OnSelectGimmick += OnSelectGimmickInFolder;
    }

    private void Update()
    {
        for (KeyCode i = KeyCode.Alpha0; i <= KeyCode.Alpha9; ++i)
        {
            if (Input.GetKeyDown(i))
            {
                int index = i - KeyCode.Alpha0;
                currentIconIndex = index;
                SelectIcon(btnQuickSlotList[index]);
                break;
            }
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            currentIconIndex = ++currentIconIndex % QuickSlotMaxCount;
            SelectIcon(btnQuickSlotList[currentIconIndex]);
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            --currentIconIndex;
            if (currentIconIndex < 0) currentIconIndex = QuickSlotMaxCount - 1;
            SelectIcon(btnQuickSlotList[currentIconIndex]);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentIconIndex = 1;
            SelectIcon(null);
            placementSystem.StartRemoving();
        }
    }

    private void SelectIcon(GimmickQuickSlotIcon _icon)
    {
        currentIcon?.Select(false);
        currentIcon = _icon;
        currentIcon?.Select(true);   
    }

    private void OnClickSlotIcon(GimmickQuickSlotIcon _icon)
    {
        if (_icon == null) return;
        
        currentIconIndex = _icon.Index;
        SelectIcon(_icon);

        if (_icon.PrefabAddress.IsNullOrEmpty()) return;
        placementSystem.StartPlacement(_icon.PrefabAddress);
    }
    
    private void OnSelectGimmickInFolder(GimmickFolderIcon _obj)
    {
        if (currentIcon == null) return;
        if (slotData.Contains(_obj.AddressPath))
        {
            // 이미 다른 슬롯에서 가지고 있는 데이터를 또 다른 슬롯에 넣으려고 한다면
            int index = slotData.FindIndex(_ => _ == _obj.AddressPath);
            
            // 이미 동일한 위치에 동일한 데이터를 넣으려고 한다면 리턴
            if (currentIconIndex == index) return;
            
            // 동일한 데이터를 가지고 있는 다른 슬롯을 비운다.
            btnQuickSlotList[index].Set(null);
            slotData[index] = string.Empty;
        }
        currentIcon.Set(_obj.AddressPath);
        slotData[currentIconIndex] = _obj.AddressPath;
    }
}
