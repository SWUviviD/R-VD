using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private float fadeInOutTime = 0.5f;

    [SerializeField] private Image blackBg;
    [SerializeField] private float endAlpha;

    [SerializeField] private Image frontBg;
    [SerializeField] private Color frontBgEndColor;

    [SerializeField] private Image clearText;
    [SerializeField] private Color clearTextEndColor;

    [SerializeField] private Button restartBtn;
    [SerializeField] private Button exitBtn;

    private bool isPlayed = false;

    private void Awake()
    {
        ResetUI();
        UIHelper.OnClick(restartBtn, GameManager.Instance.GameRestart);
        UIHelper.OnClick(exitBtn, Exit);
        UIHelper.OnClick(restartBtn, ResetUI);
        isPlayed = false;
    }

    private void Exit()
    {
        GameManager.Instance.ResetStageGameData();
        GameManager.Instance.GameExit(false);
    }


    private void Update()
    {
        if (isPlayed == false && GameManager.Instance.IsGameOver)
        {
            isPlayed = true;
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

        restartBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
    }

    private IEnumerator CoStageClear()
    {
        yield return FadeAlpha(blackBg, endAlpha);

        yield return FadeColor(frontBg, frontBgEndColor);

        yield return FadeColor(clearText, clearTextEndColor);

        restartBtn.gameObject.SetActive(true);
        exitBtn.gameObject.SetActive(true);
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
}
