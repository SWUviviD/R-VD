using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISampleToastPopup : UIBase
{
    [Header("Toast Popup Values")]
    [SerializeField] private float alphaSpeed = 3f;
    [SerializeField] private float popupTime = 3f;

    private CanvasGroup canvasGroup;
    private Text contentText;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        contentText = GetComponentInChildren<Text>();
    }

    public override void OnLoad()
    {
        gameObject.SetActive(true);
        StartCoroutine(IAlertToastPopup(alphaSpeed, popupTime));
    }

    /// <summary> 토스트 팝업 내용 생성 </summary>
    public void SetToastPopupText(string content)
    {
        contentText.text = content;
    }

    /// <summary> 토스트 팝업 이동 </summary>
    private IEnumerator IAlertToastPopup(float alphaSpeed, float popupTime)
    {
        // 생성 및 대기
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(popupTime);

        // 소멸
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
