using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AchieveUI : MonoSingleton<AchieveUI>
{
    [SerializeField] private float preWait = 1.5f;
    [SerializeField] private float duringWait = 1.5f;
    [SerializeField] private float moveTime = 0.5f;
    private WaitForSeconds wfPre;

    [SerializeField] private Image bg;
    [SerializeField] private CanvasGroup group;

    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI typeTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private Text descTxt;

    private bool isShowing = false;

    private AchieveData data = null;

    private void Start()
    {
        wfPre = new WaitForSeconds(preWait);
        bg.gameObject.SetActive(false);
    }

    public void Achieve(AchieveData data)
    {
        data?.callback?.Invoke();
    }

    public bool ShowUI(AchieveData data)
    {
        this.data = data;
        return ShowUI(duringWait, data.image, data.type, data.title, data.Desc);
    }

    public void StopUI(Action _callback = null)
    {
        if (data == null) return;

        StartCoroutine(CoMoveAndChangeAlpha(false, group,
            () => {
                isShowing = false;
                bg.gameObject.SetActive(false);
                data.callback.Invoke();
                _callback?.Invoke();
            }));
    }

    public bool ShowUI(float showStayTime, Sprite sprite, string type, string title, string desc)
    {
        if (isShowing == true)
            return false;

        isShowing = true;
        StartCoroutine(CoPreWait(showStayTime, sprite, type, title, desc));

        return true;
    }

    private IEnumerator CoPreWait(float showStayTime, Sprite sprite, string type, string title, string desc)
    {
        yield return wfPre;

        bg.gameObject.SetActive(true);
        SetUI(sprite, type, title, desc);

        StartCoroutine(CoMoveAndChangeAlpha(true, bg,
            () =>
            {
                StartCoroutine(CoMoveAndChangeAlpha(true, group,
                    () => StartCoroutine(CoWaitForFadeOut(showStayTime))));
            }));
    }

    private void SetUI(Sprite sprite, string type, string title, string desc)
    {
        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
        
        typeTxt.text = type;
        titleTxt.text = title;
        descTxt.text = desc;
    }

    private IEnumerator CoMoveAndChangeAlpha(bool fadeIn, Image image, Action _callback = null)
    {
        float elpasedTime = 0f;

        Color color = image.color;
        float start = fadeIn ? 0f : 1f;
        float end = fadeIn ? 1f : 0f;

        while (elpasedTime < moveTime)
        {
            elpasedTime += Time.deltaTime;

            color.a = Mathf.Lerp(start, end, elpasedTime / moveTime);
            image.color = color;

            yield return null;
        }

        color.a = end;
        image.color = color;

        _callback?.Invoke();
    }

    private IEnumerator CoMoveAndChangeAlpha(bool fadeIn, CanvasGroup group, Action _callback = null)
    {
        float elpasedTime = 0f;
        float start = fadeIn ? 0f : 1f;
        float end = fadeIn ? 1f : 0f;

        while (elpasedTime < moveTime)
        {
            elpasedTime += Time.deltaTime;

            group.alpha = Mathf.Lerp(start, end, elpasedTime / moveTime);

            yield return null;
        }

        group.alpha = end;

        _callback?.Invoke();
    }

    private IEnumerator CoWaitForFadeOut(float time, Action callback = null)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
}
