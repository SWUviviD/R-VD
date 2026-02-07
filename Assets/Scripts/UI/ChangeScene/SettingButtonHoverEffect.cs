using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected float hoverScale = 1.2f;
    [SerializeField] protected Image btnImage;
    [SerializeField] protected Sprite defaultSprite;
    [SerializeField] protected Sprite hoveredSprite;

    private Vector3 originalScale;
    private Color originalColor;
    private Color curColor;

    private void Awake()
    {
        if (btnImage == null)
        {
            this.enabled = false;
            return;
        }

        originalScale = transform.localScale;
        originalColor = btnImage.color;
        curColor = originalColor;
    }

    protected virtual void OnEnable()
    {
        OnPointerExit(null);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        if (btnImage) btnImage.sprite = hoveredSprite;
        curColor.a = hoveredSprite == null ? 0f : originalColor.a;
        btnImage.color = curColor;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        if (btnImage) btnImage.sprite = defaultSprite;
        curColor.a = defaultSprite == null ? 0f : originalColor.a;
        btnImage.color = curColor;
    }
}