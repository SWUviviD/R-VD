using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIHelper
{
    public static void OnClick(Button _btn, UnityAction _callback)
    {
        //#if UNITY_EDITOR
        //UnityEventTools.RemovePersistentListener(_btn.onClick, _callback);
        //UnityEventTools.AddPersistentListener(_btn.onClick, _callback);
        //#else
        _btn.onClick.RemoveListener(_callback);
        _btn.onClick.AddListener(_callback);
        //#endif
    }
}
