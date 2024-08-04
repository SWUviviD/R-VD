using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] private UIDefines.UIType uiType;

    public UIDefines.UIType UIType => uiType;
    
    /// <summary>
    /// UI의 생성이 완료됨
    /// OnEnable 이후에 호출
    /// </summary>
    public virtual void OnLoad()
    {
        
    }
    
    /// <summary>
    /// 현재 팝업을 닫는다.
    /// </summary>
    public void Close()
    {
        UIManager.Instance.Close(this);
    }

    /// <summary>
    /// 자식 팝업이 삭제된경우 호출
    /// </summary>
    public virtual void OnChildPopupClose()
    {
        
    }
}
