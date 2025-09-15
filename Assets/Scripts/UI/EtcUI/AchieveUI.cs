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
    [SerializeField] private float moveTime = 0.5f;
    private WaitForSeconds wfPre;

    [SerializeField] private Image bg;
    [SerializeField] private CanvasGroup group;

    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI typeTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private Text descTxt;

    private bool isShowing = false;

    private void Start()
    {
        wfPre = new WaitForSeconds(preWait);
        bg.gameObject.SetActive(false);
    }

    public bool ShowUI(AchieveData data, Action _callback = null)
    {
        data.callback.AddListener(() => _callback?.Invoke());
        return ShowUI(data.duration, data.image, data.type, data.title, data.Desc, data.callback);
    }

    public bool ShowUI(float showStayTime, Sprite sprite, string type, string title, string desc, UnityEvent _callback = null)
    {
        if (isShowing == true)
            return false;

        isShowing = true;
        StartCoroutine(CoPreWait(showStayTime, sprite, type, title, desc, _callback));

        return true;
    }

    private IEnumerator CoPreWait(float showStayTime, Sprite sprite, string type, string title, string desc, UnityEvent _callback = null)
    {
        yield return wfPre;

        bg.gameObject.SetActive(true);
        SetUI(sprite, type, title, desc);

        StartCoroutine(CoMoveAndChangeAlpha(true, bg,
            () =>
            {
                StartCoroutine(CoMoveAndChangeAlpha(true, group,
                    () => StartCoroutine(CoWaitForFadeOut(showStayTime,
                    () => StartCoroutine(CoMoveAndChangeAlpha(false, group,
                    () =>
                    {
                        isShowing = false;
                        bg.gameObject.SetActive(false);
                        _callback?.Invoke();
                    }))))));
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

    private IEnumerator CoWaitForFadeOut(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback.Invoke();
    }
}
