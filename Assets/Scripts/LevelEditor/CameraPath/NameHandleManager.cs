using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 이름을 보여주는 핸들을 관리하는 매니저
/// 화면에 보이는 핸들만 UI를 표기하며, 화면에서 벗어나면 UI를 회수한다.
/// </summary>
public class NameHandleManager : MonoSingleton<NameHandleManager>
{
    /// <summary>
    /// 카메라 핸들 프리팹
    /// </summary>
    [SerializeField] UINameHandle prefabNameHandle;

    [SerializeField] RectTransform rcCanvas; 
    
    /// <summary>
    /// 생성된 핸들을 관리하는 리스트. 반환된 핸들은 여기에 추가된다.
    /// </summary>
    private List<UINameHandle> pathHandleList;

    public RectTransform CanvasRectTransform => rcCanvas;

    private void Start()
    {
        pathHandleList = new List<UINameHandle>();
    }

    public UINameHandle GetHandleUI(Transform _trTarget, NameHandleData _data)
    {
        UINameHandle handle;
        if (pathHandleList.Count == 0)
        {
            handle = CreateHandle();
        }
        else
        {
            handle = pathHandleList[^1];
            pathHandleList.RemoveAt(pathHandleList.Count - 1);
        }

        handle.WakeUp(_trTarget, _data);
        return handle;
    }

    private UINameHandle CreateHandle()
    {
        UINameHandle handle = Instantiate(prefabNameHandle, transform);
        handle.Initialize(ReturnToPool);
        return handle;
    }

    private void ReturnToPool(UINameHandle _handle)
    {
        pathHandleList.Add(_handle);
    }
}
