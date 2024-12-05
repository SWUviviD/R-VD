using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale; // 버튼의 원래 크기
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Entered");
        transform.localScale = originalScale * 1.5f; // 크기를 1.5배로 키움
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // 원래 크기로 복구
    }
}