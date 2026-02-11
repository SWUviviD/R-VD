using System;
using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Transform siroTalkUIEndPos;

    [Header("TutorialUI")]
    [SerializeField] private CanvasGroup TutorialUI;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Transform tutorialUIEndPos;

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
        currentTargetCount = 0;

        textPandingTime = maxTextTime / maxTextCount;

        wfShowNextTextPanding = new WaitForSeconds(showNextTextPandingTime);
        wfLoadOut = new WaitForSeconds(loadOutTime);
        wfTutorialMaxShow = new WaitForSeconds(waitForTutorialMaxShowTime);

        SceneLoadManager.Instance.onSceneUnloaded_permanent.AddListener((Scene) => StopShowTutorial());
    }

    public void PlayTutorialTxt(TutorialInfo info, TutorialStartTrigger trigger, int curCount)
    {
        StopAllCoroutines();
        ResetUI();

        currentTrigger = trigger;
        SetInfo(info, curCount);
        if(currentTargetCount >= info.MaxCount)
        {
            currentTrigger.isDone = true;

            StopAllCoroutines();

            state = TutorialState.TutorialCountEnd;
            StartCoroutine(CoWaitAndEndTutorial());

            return;
        }

        StartCoroutine(CoLoadIn(SiroTalk, siroTalkUIEndPos.localPosition, () => ShowNextText(0)));

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
            currentTrigger.isDone = true;

            StopAllCoroutines();

            StartCoroutine(CoWaitAndEndTutorial());
        }
    }

    private void TutorialEnd()
    {
        StartCoroutine(CoLoadOut(TutorialUI, tutorialUIEndPos.localPosition, ResetUI));
        state = TutorialState.TutorialCountEnd;
    }

    public void StopShowTutorial()
    {
        currentInfo = null;
        currentTalkLine = -1;
        currentTargetCount = 0;

        state = TutorialState.NONE;

        SiroTalk.transform.localPosition = AnimationStartPoint.localPosition;
        SiroTalk.alpha = 0f;
        talkText.text = string.Empty;

        TutorialUI.transform.localPosition = AnimationStartPoint.localPosition;
        TutorialUI.alpha = 0f;
        targetText.text = string.Empty;
        countText.text = string.Empty;
    }

    private void ResetUI()
    {
        currentInfo = null;
        currentTalkLine = -1;
        currentTargetCount = 0;

        state = TutorialState.NONE;

        SiroTalk.transform.localPosition = AnimationStartPoint.localPosition;
        SiroTalk.alpha = 1.0f;
        talkText.text = string.Empty;

        TutorialUI.transform.localPosition = AnimationStartPoint.localPosition;
        TutorialUI.alpha = 1.0f;
        targetText.text = string.Empty;
        countText.text = string.Empty;
    }

    private void SetInfo(TutorialInfo info, int count)
    {
        currentInfo = info;
        currentTalkLine = 0;
        currentTargetCount = count;

        // Siro

        // Tutorial
        TutorialUI.gameObject.SetActive(false);
        TutorialUI.gameObject.SetActive(true);
        targetText.text = info.TutorialTargetTxt;
        countText.text = $"({currentTargetCount}/{info.MaxCount})";
    }

    private IEnumerator CoLoadIn(CanvasGroup panel, Vector3 endPos, Action callback = null)
    {
        float elapsedTime = 0f;

        panel.alpha = 1.0f;
        Vector3 startPos = AnimationStartPoint.localPosition;
        Vector3 curPos = startPos;
        panel.gameObject.SetActive(true);

        while (elapsedTime < loadInTime)
        {
            curPos = Vector3.Lerp(startPos, endPos, elapsedTime / loadInTime);
            panel.transform.localPosition = curPos;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        panel.transform.localPosition = endPos;

        callback?.Invoke();
    }

    private IEnumerator CoLoadOut(CanvasGroup panel, Vector3 panelOriginPos, Action callback)
    {
        float elapsedTime = 0f;
        float alpha = 0.0f;

        panel.transform.localPosition = panelOriginPos;
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
            StartCoroutine(CoLoadOut(SiroTalk, siroTalkUIEndPos.localPosition, StartTutorialPanding));
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

        StartCoroutine(CoLoadIn(TutorialUI, tutorialUIEndPos.localPosition));
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
            yield return CoWaitForScaledSeconds(textPandingTime);
        }

        yield return CoWaitForScaledSeconds(showNextTextPandingTime);
        _callback?.Invoke();
    }

    private IEnumerator CoWaitForScaledSeconds(float time)
    {
        float restTime = 0f;
        while (restTime < time)
        {
            yield return null;
            restTime += Time.deltaTime;
        }
    }

    private IEnumerator CoWaitAndEndTutorial()
    {
        SiroTalk.gameObject.SetActive(false);

        TutorialUI.gameObject.SetActive(true);
        TutorialUI.transform.localPosition = tutorialUIEndPos.localPosition;
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
