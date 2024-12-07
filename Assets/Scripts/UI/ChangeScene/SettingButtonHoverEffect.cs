using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoveredSprite;

    private Vector3 originalScale;
    private Image image;

    private void Start()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        image.sprite = hoveredSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        image.sprite = defaultSprite;
    }
}