using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UINameHandle이 타깃을 따라가도록 하는 로직
/// NameHandleData를 설정해주지 않으면 이름을 표기할 수 없으므로, 사용하기 위해선 NameHandleData를 설정해야한다.
/// </summary>
public class NameHandleTarget : MonoBehaviour
{
    private UINameHandle nameHandle;
    private NameHandleData handleData;
    private bool hasNameHandle;

    private Camera camera;
    private RectTransform rcCanvas;
    
    private void Awake()
    {
        nameHandle = null;
        handleData = null;
        hasNameHandle = false;
        camera = Camera.main;
        rcCanvas = NameHandleManager.Instance.CanvasRectTransform;
    }

    public void Set(NameHandleData _data)
    {
        handleData = _data;
    }

    private void Update()
    {
        if (handleData == null) return;

        if (IsInCamera() && hasNameHandle == false)
        {
            nameHandle = NameHandleManager.Instance.GetHandleUI(transform, handleData);
            hasNameHandle = true;
        }
        else if (IsInCamera() == false && hasNameHandle)
        {
            nameHandle.Sleep();
            nameHandle = null;
            hasNameHandle = false;
        }

        if (hasNameHandle == false) return;

        Vector2 viewport = camera.WorldToViewportPoint(transform.position);
        Vector2 screenPosition = new Vector2(
            viewport.x * rcCanvas.sizeDelta.x - rcCanvas.sizeDelta.x * 0.5f,
            viewport.y * rcCanvas.sizeDelta.y - rcCanvas.sizeDelta.y * 0.5f);

        (nameHandle.transform as RectTransform).anchoredPosition = screenPosition;
    }

    private bool IsInCamera()
    {
        Vector2 viewPort = camera.WorldToViewportPoint(transform.position);
        return viewPort.x > 0f && viewPort.x < 1f &&
               viewPort.y > 0f && viewPort.y < 1f;
    }
}
