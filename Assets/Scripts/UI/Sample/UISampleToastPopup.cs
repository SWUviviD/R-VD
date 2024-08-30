using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISampleToastPopup : UIBase
{

    [Header("Toast Popup Values")]
    [SerializeField] private float appearSpeed = 3f;
    [SerializeField] private float holdingTime = 3f;
    [SerializeField] private float disappearSpeed = 3f;

    private RectTransform rectTR;
    private CanvasGroup canvasGroup;
    private Text contentText;

    private Vector3 beforePos;
    private Vector3 afterPos;
    private float rectWidth;
    private float rectHeight;

    private void Awake()
    {
        rectTR = GetComponent<RectTransform>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        contentText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        rectWidth = rectTR.rect.width;
        rectHeight = rectTR.rect.height;

        beforePos = rectTR.position + Vector3.down * (rectTR.position.y + rectHeight);
        afterPos = rectTR.position;
        rectTR.position = beforePos;
    }

    public override void OnLoad()
    {
        gameObject.SetActive(true);
        StartCoroutine(IAlertToastPopup(appearSpeed, holdingTime, disappearSpeed));
    }

    /// <summary> 토스트 팝업 내용 생성 </summary>
    public void SetToastPopupText(string content)
    {
        contentText.text = content;
    }

    /// <summary> 토스트 팝업 이동 </summary>
    private IEnumerator IAlertToastPopup(float appearSpeed, float holdTime, float disappearSpeed)
    {
        // 생성
        canvasGroup.alpha = 1f;
        while (rectTR.position != afterPos)
        {
            rectTR.position = Vector3.MoveTowards(rectTR.position, afterPos, appearSpeed);
            yield return null;
        }

        // 대기
        yield return new WaitForSeconds(holdTime);

        // 소멸
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= disappearSpeed * Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
