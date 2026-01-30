using System;
using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class TutorialPlayer : MonoSingleton<TutorialPlayer>
{
    private enum TutorialState
    {
        NONE = -1,
        SiroTalking,
        SiroTalkEnding,
        TutorialCounting,
        TutorialCountEnd,
    }

    [SerializeField] private Transform AnimationStartPoint;

    [Header("SiroTalkUI")]
    [SerializeField] private CanvasGroup SiroTalk;
    [SerializeField] private TextMeshProUGUI talkText;
    [SerializeField] private int maxTextCount = 34;
    private Vector3 siroTalkUIEndPos = Vector3.zero;

    [Header("TutorialUI")]
    [SerializeField] private CanvasGroup TutorialUI;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI countText;
    private Vector3 tutorialUIEndPos = Vector3.zero;

    [Header("Timing")]
    [SerializeField] private float loadInTime = 0.3f;
    [SerializeField] private float maxTextTime = 1.5f;
    private float textPandingTime = 0f;
    [SerializeField] private float showNextTextPandingTime = 2f;
    private WaitForSeconds wfShowNextTextPanding;
    [SerializeField] private float loadOutTime = 0.3f;
    private WaitForSeconds wfLoadOut;
    [SerializeField] private float waitForTutorialMaxShowTime = 0.5f;
    private WaitForSeconds wfTutorialMaxShow;

    private TutorialState state;

    private TutorialStartTrigger currentTrigger;
    private TutorialInfo currentInfo;
    private int currentTalkLine = 0;
    private int currentTargetCount = 0;

    protected override void Init()
    {
        base.Init();

        state = TutorialState.NONE;

        siroTalkUIEndPos = SiroTalk.transform.position;
        tutorialUIEndPos = TutorialUI.transform.position;

        textPandingTime = maxTextTime / maxTextCount;

        wfShowNextTextPanding = new WaitForSeconds(showNextTextPandingTime);
        wfLoadOut = new WaitForSeconds(loadOutTime);
        wfTutorialMaxShow = new WaitForSeconds(waitForTutorialMaxShowTime);
    }

    public void PlayTutorialTxt(TutorialInfo info, TutorialStartTrigger trigger)
    {
        StopAllCoroutines();
        ResetUI();

        currentTrigger = trigger;
        SetInfo(info);
        StartCoroutine(CoLoadIn(SiroTalk, siroTalkUIEndPos, () => ShowNextText(0)));

        state = TutorialState.SiroTalking;
    }

    public void TargetAchieved(TutorialStartTrigger trigger)
    {
        if (currentInfo == null)
            return;

        if (currentTrigger != trigger)
            return;

        currentTargetCount += 1;
        countText.text = $"({currentTargetCount}/{currentInfo.MaxCount})";
        if (currentTargetCount >= currentInfo.MaxCount)
        {
            StopAllCoroutines();

            StartCoroutine(CoWaitAndEndTutorial());
        }
    }

    private void TutorialEnd()
    {
        StartCoroutine(CoLoadOut(TutorialUI, tutorialUIEndPos, ResetUI));
        state = TutorialState.TutorialCountEnd;
    }

    private void ResetUI()
    {
        currentInfo = null;
        currentTalkLine = -1;
        currentTargetCount = 0;

        state = TutorialState.NONE;

        SiroTalk.transform.position = AnimationStartPoint.transform.position;
        SiroTalk.alpha = 1.0f;
        talkText.text = string.Empty;

        TutorialUI.transform.position = AnimationStartPoint.transform.position;
        TutorialUI.alpha = 1.0f;
        targetText.text = string.Empty;
        countText.text = string.Empty;
    }

    private void SetInfo(TutorialInfo info)
    {
        currentInfo = info;
        currentTalkLine = 0;
        currentTargetCount = 0;

        // Siro

        // Tutorial
        TutorialUI.gameObject.SetActive(false);
        TutorialUI.gameObject.SetActive(true);
        targetText.text = info.TutorialTargetTxt;
        countText.text = $"(0/{info.MaxCount})";
    }

    private IEnumerator CoLoadIn(CanvasGroup panel, Vector3 endPos, Action callback = null)
    {
        float elapsedTime = 0f;

        panel.alpha = 1.0f;
        Vector3 startPos = AnimationStartPoint.transform.position;
        Vector3 curPos = startPos;
        panel.gameObject.SetActive(true);

        while (elapsedTime < loadInTime)
        {
            curPos = Vector3.Lerp(startPos, endPos, elapsedTime / loadInTime);
            panel.transform.position = curPos;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        callback?.Invoke();
    }

    private IEnumerator CoLoadOut(CanvasGroup panel, Vector3 panelOriginPos, Action callback)
    {
        float elapsedTime = 0f;
        float alpha = 0.0f;

        panel.transform.position = panelOriginPos;
        panel.gameObject.SetActive(true);

        while (elapsedTime < loadOutTime)
        {
            alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / loadOutTime);
            panel.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.alpha = 0f;
        panel.gameObject.SetActive(false);

        callback?.Invoke();
    }

    private void ShowNextText(int lineNum)
    {
        if (currentInfo == null)
            return;

        if(lineNum == currentInfo.talkLines.Length)
        {
            StartCoroutine(CoLoadOut(SiroTalk, siroTalkUIEndPos, StartTutorialPanding));
            state = TutorialState.SiroTalkEnding;
            return;
        }

        StartTextShowing(currentInfo.talkLines[lineNum].Text,
            () => ShowNextText(lineNum + 1));
    }

    private void StartTutorialPanding()
    {
        if (currentInfo == null)
            return;

        if(currentTargetCount >= currentInfo.MaxCount)
        {
            StopAllCoroutines();

            StartCoroutine(CoWaitAndEndTutorial());
            return;
        }

        StartCoroutine(CoLoadIn(TutorialUI, tutorialUIEndPos));
    }

    private void StartTextShowing(string text, Action _callback = null)
    {
        StartCoroutine(CoShowText(text, _callback));
    }

    private IEnumerator CoShowText(string text, Action _callback = null)
    {
        var builder = new System.Text.StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            builder.Append(text[i]);
            talkText.text = builder.ToString();
            yield return CoWaitForUnscaledSeconds(textPandingTime);
        }

        yield return CoWaitForUnscaledSeconds(showNextTextPandingTime);
        _callback?.Invoke();
    }

    private IEnumerator CoWaitForUnscaledSeconds(float time)
    {
        float restTime = 0f;
        while (restTime < time)
        {
            yield return null;
            restTime += Time.unscaledDeltaTime;
        }
    }

    private IEnumerator CoWaitAndEndTutorial()
    {
        SiroTalk.gameObject.SetActive(false);

        TutorialUI.gameObject.SetActive(true);
        TutorialUI.transform.position = tutorialUIEndPos;
        TutorialUI.alpha = 1f;

        countText.text = $"({currentTargetCount}/{currentInfo.MaxCount})";

        state = TutorialState.TutorialCounting;

        if (state == TutorialState.TutorialCountEnd)
            yield break;

        state = TutorialState.TutorialCounting;

        yield return wfTutorialMaxShow;

        TutorialEnd();
    }

}
