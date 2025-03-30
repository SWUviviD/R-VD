using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private Image btnImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoveredSprite;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        if (btnImage) btnImage.sprite = hoveredSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        if (btnImage) btnImage.sprite = defaultSprite;
    }
}