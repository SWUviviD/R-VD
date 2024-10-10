using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GimmickQuickSlotIcon : MonoBehaviour
{
    [SerializeField] private Button btnIcon;
    [SerializeField] private GameObject objSelected;
    [SerializeField] private Text txtName;

    /// <summary> 프리팹의 경로 </summary>
    public string PrefabAddress { get; private set; }
    public bool IsSelected => objSelected.activeSelf;
    public string PrefabName => txtName.text;
    public int Index { get; private set; }
    
    public void Init(System.Action<GimmickQuickSlotIcon> _cbClick, int _index)
    {
        Index = _index;
        btnIcon.onClick.AddListener(() => _cbClick(this));
    }

    public void Set(string _address)
    {
        if (_address.IsNullOrEmpty())
        {
            PrefabAddress = string.Empty;
        }
        else
        {
            // 확장자를 제거한 파일의 이름을 가져온다.
            PrefabAddress = _address.Split("/")[^1].Split(".")[0];
        }

        txtName.text = PrefabAddress;
    }

    public void Select(bool _set)
    {
        objSelected.SetActive(_set);
    }
}
