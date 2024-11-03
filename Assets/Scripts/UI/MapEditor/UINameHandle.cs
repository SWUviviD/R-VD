using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UINameHandle을 사용할때 필요한 데이터
/// </summary>
public class NameHandleData
{
    public string Name { get; set; }
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }

    public Action OnClick { get; set; }

    public NameHandleData(string _name, Color _backgroundColor, Color _textColor, Action _onClick)
    {
        Name = _name;
        BackgroundColor = _backgroundColor;
        TextColor = _textColor;
        OnClick = _onClick;
    }
}

/// <summary>
/// 화면에 이름을 표기하는 핸들.
/// 실질적으로 이름을 표기하기 위해 타깃을 따라가는 로직은 NameHandleTarget에 있다. 
/// </summary>
public class UINameHandle : MonoBehaviour
{
    /// <summary> 핸들의 버튼 </summary>
    [SerializeField] private Button btnHandle;
    /// <summary> 핸들 이미지 </summary>
    [SerializeField] private Image imgHandle;
    /// <summary> 핸들의 이름 </summary>
    [SerializeField] private Text txtHandle;
    
    /// <summary>
    /// 따라가야할 대상의 트랜스폼
    /// </summary>
    private Transform trTarget;

    private NameHandleData handleData;

    /// <summary>
    /// 풀로 되돌아가는 함수
    /// </summary>
    private System.Action<UINameHandle> cbReturnToPool;

    /// <summary> 캐싱해둔 메인 카메라 </summary>
    private Camera camera;
    
    public void Initialize(Action<UINameHandle> _cbReturnToPool)
    {
        UIHelper.OnClick(btnHandle, OnClickHandle);
        cbReturnToPool = _cbReturnToPool;
        camera = Camera.main;
    }

    public void WakeUp(Transform _trTarget, NameHandleData _data)
    {
        handleData = _data;
        trTarget = _trTarget;
        imgHandle.color = _data.BackgroundColor;
        txtHandle.text = _data.Name;
        txtHandle.color = _data.TextColor;
        gameObject.SetActive(true);
    }
    
    public void Sleep()
    {
        trTarget = null;
        cbReturnToPool(this);
        gameObject.SetActive(false);
    }

    private void OnClickHandle()
    {
        handleData.OnClick?.Invoke();
    }
}
