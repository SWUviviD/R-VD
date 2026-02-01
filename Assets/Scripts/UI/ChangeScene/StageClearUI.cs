using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : MonoBehaviour
{
    [SerializeField] private float fadeInOutTime = 0.5f;

    [SerializeField] private Image blackBg;
    [SerializeField] private float endAlpha;

    [SerializeField] private Image frontBg;
    [SerializeField] private Color frontBgEndColor;

    [SerializeField] private Image clearText;
    [SerializeField] private Color clearTextEndColor;

    [SerializeField] private Button NextBtn;

    private bool isPlaying = false;

    private void Awake()
    {
        ResetUI();
        UIHelper.OnClick(NextBtn, ResetUI);
    }

    private void Update()
    {
        if (!isPlaying && GameManager.Instance.IsStageClear)
        {
            isPlaying = true;
            StartCoroutine(CoStageClear());
        }
    }

    public void ResetUI()
    {
        SetAlpha(blackBg, 0f);

        frontBg.color = new Color(
            frontBgEndColor.r,
            frontBgEndColor.g,
            frontBgEndColor.b,
            0f);

        clearText.color = new Color(
            clearTextEndColor.r,
            clearTextEndColor.g,
            clearTextEndColor.b,
            0f);

        NextBtn.gameObject.SetActive(false);
        isPlaying = false;
    }

    private IEnumerator CoStageClear()
    {
        yield return FadeAlpha(blackBg, endAlpha);

        yield return FadeColor(frontBg, frontBgEndColor);

        yield return FadeColor(clearText, clearTextEndColor);

        NextBtn.gameObject.SetActive(true);
    }

    // ===== Utils =====

    private IEnumerator FadeAlpha(Image img, float targetAlpha)
    {
        float startAlpha = img.color.a;
        float time = 0f;

        Color c = img.color;

        while (time < fadeInOutTime)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeInOutTime);
            img.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        img.color = c;
    }

    private IEnumerator FadeColor(Image img, Color target)
    {
        Color start = img.color;
        float time = 0f;

        while (time < fadeInOutTime)
        {
            time += Time.deltaTime;
            img.color = Color.Lerp(start, target, time / fadeInOutTime);
            yield return null;
        }

        img.color = target;
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    //private void Awake()
    //{
    //    // TODO: 다음 스테이지 연결//GameManager.Instance.NextStage);
    //    UIHelper.OnClick(quitButton, GameManager.Instance.LoadTitle);
    //    isCleared = false;
    //}

    //private void Start()
    //{
    //    ResetUI();
    //}

    //public void ResetUI()
    //{
    //    whiteOverlay.color = new Color(whiteOverlay.color.r, whiteOverlay.color.g, whiteOverlay.color.b, 0f);
    //    bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0f);

    //    // 텍스트를 처음엔 투명하게 설정
    //    Color textColor2 = stageText.color;
    //    textColor2.a = 0;
    //    stageText.color = textColor2;

    //    Color textColor1 = stageClearText.color;
    //    textColor1.a = 0;
    //    stageClearText.color = textColor1;

    //    // 버튼 초기 비활성화
    //    nextButton.gameObject.SetActive(false);
    //}

    //private void Update()
    //{
    //    if (isCleared)
    //        return;

    //    if (GameManager.Instance.IsStageClear)
    //    {
    //        HandleBlackOverlayFade();
    //    }
    //    else if (whiteOverlay.color.a != 0)
    //    {
    //        Color overlayColor = whiteOverlay.color;
    //        overlayColor.a = 0;
    //        whiteOverlay.color = overlayColor;

    //        nextButton.gameObject.SetActive(false);
    //        Color textColor1 = stageClearText.color;
    //        textColor1.a = 0;
    //        stageClearText.color = textColor1;
    //        Color textColor2 = stageText.color;
    //        textColor2.a = 0;
    //        stageText.color = textColor2;
    //    }
    //}

    //private void HandleBlackOverlayFade()
    //{
    //    Color overlayColor = whiteOverlay.color;

    //    if (overlayColor.a < backgroundAlpha)
    //    {
    //        overlayColor.a += Time.deltaTime * fadeSpeed;
    //        whiteOverlay.color = overlayColor;

    //        if (overlayColor.a >= backgroundAlpha - 0.2f && !nextButton.gameObject.activeSelf)
    //        {
    //            //if (!GameManager.Instance.IsLastScene)
    //            //    nextButton.gameObject.SetActive(true);
    //            //else
    //            //    quitButton.gameObject.SetActive(true);

    //            isCleared = true;
    //            nextButton.gameObject.SetActive(true);

    //            Color textColor1 = stageClearText.color;
    //            textColor1.a = 1;
    //            stageClearText.color = textColor1;
    //            Color textColor2 = stageText.color;
    //            textColor2.a = 1;
    //            stageText.color = textColor2;
    //        }
    //    }
    //}
}
