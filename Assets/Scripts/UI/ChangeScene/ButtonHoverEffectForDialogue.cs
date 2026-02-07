using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHoverEffectForDialogue : SettingButtonHoverEffect
{
    [SerializeField] private UnityEvent OnHoverExitEvent;
    [SerializeField] private UnityEvent OnHoverEvent;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (btnImage) btnImage.sprite = hoveredSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); 
        if (btnImage) btnImage.sprite = hoveredSprite;
        OnHoverEvent?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (btnImage) btnImage.sprite = hoveredSprite;
        OnHoverExitEvent?.Invoke();
    }
}
