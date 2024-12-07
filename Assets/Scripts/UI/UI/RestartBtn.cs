using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RestartBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform btnHoverImage;
    private Image hoberImg;

    private void Awake()
    {
        hoberImg = btnHoverImage?.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        btnHoverImage?.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        btnHoverImage?.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
