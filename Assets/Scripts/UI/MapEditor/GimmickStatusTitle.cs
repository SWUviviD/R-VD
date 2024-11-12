using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GimmickStatusTitle : MonoBehaviour, IDragHandler
{
    public Action<Vector2> OnDragTitle { get; set; }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragTitle(eventData.delta);
    }
}
