using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffectForTitle : SettingButtonHoverEffect
{
    [SerializeField] private Color hoverImageColor;

    [Header("Text")]
    [SerializeField] private Text btnText;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;

    protected override void OnEnable()
    {
        base.OnEnable();
        btnText.color = defaultColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        btnImage.color = hoverImageColor;
        btnText.color = hoverColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        btnText.color = defaultColor;
    }
}
