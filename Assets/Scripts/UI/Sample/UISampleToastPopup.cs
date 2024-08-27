using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISampleToastPopup : UIBase
{
    [Space]
    [SerializeField] private Text titleText;
    [SerializeField] private Text contentText;

    [Space]
    [SerializeField] private float popupSpeed = 5f;

    private RectTransform rectTR;
    private float rectWidth;
    private float rectHeight;

    private void Awake()
    {
        rectTR = GetComponent<RectTransform>();

        rectWidth = rectTR.rect.width;
        rectHeight = rectTR.rect.height;
    }

    public override void OnLoad()
    {
        StartCoroutine(IAlertToastPopup(popupSpeed));
    }

    /// <summary> 토스트 팝업 내용 생성 </summary>
    public void SetToastPopupText(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
    }

    /// <summary> 토스트 팝업 이동 </summary>
    private IEnumerator IAlertToastPopup(float speed)
    {
        for (int i = 0; i < rectHeight; i += (int)speed)
        {
            rectTR.position += Vector3.up * speed;
            yield return null;
        }
    }
}
