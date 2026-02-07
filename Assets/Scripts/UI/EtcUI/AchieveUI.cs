using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Defines.InputDefines;

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

    public bool IsShowing => isShowing;
    private bool isShowing = false;

    private AchieveData data = null;
    private Action endCallback = null;

    private bool canSkip = false;

    [Header("Setting")]
    [SerializeField] private TitleSettingUI settingUI;
    [SerializeField] private Image skipRoll;
    [SerializeField] private Image skipRollbg;
    [SerializeField] private float skipTime = 1f;
    private float elapsedTime = 0f;
    private bool isPressing = false;

    private void Start()
    {
        wfPre = new WaitForSeconds(preWait);
        bg.gameObject.SetActive(false);
        skipRollbg.gameObject.SetActive(false);
    }

    private void AddInputEvent()
    {
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsStarted, OnKeyPressed);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsCanceled, OnKeyCanceled);
    }

    private void RemoveInputEvent()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsStarted, OnKeyPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsCanceled, OnKeyCanceled);
    }


    private void OnKeyPressed(InputAction.CallbackContext context)
    {
        if (canSkip == false) return;

        elapsedTime = 0f;
        skipRoll.fillAmount = 0f;
        skipRollbg.gameObject.SetActive(true);
        isPressing = true;
    }

    private void OnKeyCanceled(InputAction.CallbackContext context)
    {
        if (canSkip == false) return;

        elapsedTime = 0f;
        skipRollbg.gameObject.SetActive(false);
        isPressing = false;
    }

    private void Update()
    {
        if (isPressing == true)
        {
            elapsedTime += Time.deltaTime;
            skipRoll.fillAmount = elapsedTime / skipTime;
            if (elapsedTime > skipTime)
            {
                isPressing = false;
                canSkip = false;

                StopAllCoroutines();
                RemoveInputEvent();

                StartCoroutine(CoMoveAndChangeAlpha(false, group,
                    () =>
                    {
                        StartCoroutine(CoMoveAndChangeAlpha(false, bg,
                            () =>
                            {
                                isShowing = false;
                                bg.gameObject.SetActive(false);
                                data?.callback?.Invoke();
                                endCallback?.Invoke();
                            }));
                    }));
            }
        }
    }

    public void Achieve(AchieveData data, Action callBack = null)
    {
        callBack?.Invoke();
        data?.callback?.Invoke();
    }

    public bool ShowUI(AchieveData data, Action callBack = null)
    {
        this.data = data;
        endCallback = callBack;
        canSkip = false;
        skipRollbg.gameObject.SetActive(false);
        AddInputEvent();
        return ShowUI(duringWait, data.bgImage, data.image, data.type, data.title, data.Desc);
    }

    public void StopUI(Action _callback = null)
    {
        if (data == null) return;

        StartCoroutine(CoMoveAndChangeAlpha(false, group,
            () => {
                isShowing = false;
                bg.gameObject.SetActive(false);
                data?.callback?.Invoke();
                endCallback?.Invoke();
                _callback?.Invoke();
            }));
    }

    public bool ShowUI(float showStayTime, Sprite bgSprite, Sprite sprite, string type, string title, string desc)
    {
        if (isShowing == true)
            return false;

        isShowing = true;
        StartCoroutine(CoPreWait(showStayTime, bgSprite, sprite, type, title, desc));

        return true;
    }

    private IEnumerator CoPreWait(float showStayTime, Sprite bgSprite, Sprite sprite, string type, string title, string desc)
    {
        yield return wfPre;

        bg.gameObject.SetActive(true);
        SetUI(bgSprite, sprite, type, title, desc);

        StartCoroutine(CoMoveAndChangeAlpha(true, bg,
            () => {
                StartCoroutine(CoMoveAndChangeAlpha(true, group,
                    () =>
                    {
                        canSkip = true;
                        StartCoroutine(CoWaitForFadeOut(showStayTime));
                    }));
            }));
    }

    private void SetUI(Sprite bgSprite, Sprite sprite, string type, string title, string desc)
    {
        bg.sprite = bgSprite;

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
