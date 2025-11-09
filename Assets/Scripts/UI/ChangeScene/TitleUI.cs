using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleUI : MonoBehaviour
{
    [Header("IntroEffect")]
    [SerializeField] private float BtnFadeTime = 1f;
    [SerializeField] private float moveSpeed = 50f;

    [Header("BG")]
    [SerializeField] private VideoPlayer bgPlayer;
    [SerializeField] private VideoClip TitleVideo;
    [SerializeField] private VideoClip IntroVideo;
    [SerializeField] private float introTime = 12f;
    private WaitForSeconds wfIntroVideo;

    [Header("Logo")]
    [SerializeField] private Image logoImage;

    [Header("Btn")]
    [SerializeField] private CanvasGroup btnGroup;
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button CreditBtn;
    [SerializeField] private Button SettingBtn;
    [SerializeField] private Button ExitBtn;

    [Header("Setting")]
    [SerializeField] private TitleSettingUI settingUI;

    private void Start()
    {
        btnGroup.enabled = false;

        bgPlayer.clip = TitleVideo;
        bgPlayer.isLooping = true;
        bgPlayer.playOnAwake = true;

#if UNITY_EDITOR
        wfIntroVideo = new WaitForSeconds(0.5f);
#else
        wfIntroVideo = new WaitForSeconds(introTime);
#endif

        UIHelper.OnClick(StartBtn, NewGameStart);
        UIHelper.OnClick(ExitBtn, GameExit);
        UIHelper.OnClick(CreditBtn, LoadCredit);

        if (GameManager.Instance.GameDataManager.LoadGameData() == true)
        {
            ContinueBtn.GetComponentInChildren<Text>().raycastTarget = true;
            ContinueBtn.GetComponentInChildren<Text>().color = Color.white;
            ContinueBtn.GetComponent<SettingButtonHoverEffect>().enabled = true;
            UIHelper.OnClick(ContinueBtn, GameManager.Instance.LoadData);
        }

        UIHelper.OnClick(SettingBtn, () => settingUI.ShowPanel(true));
    }

    private void NewGameStart()
    {
        //CutSceneManager.Instance.PlayCutScene(
        //    Defines.CutSceneDefines.CutSceneNumber.Intro,
        //    GameManager.Instance.NewGameStart);

        // 컷씬 말고 영상으로 대체
        bgPlayer.clip = IntroVideo;
        bgPlayer.isLooping = false;

        StartCoroutine(CoPlayIntro());
        StartCoroutine(CoOnVideoEnd());
    }

    private void LoadCredit()
    {
        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Credit);
    }

    private void GameExit()
    {
        GameManager.Instance.GameExit(false);
    }

    private IEnumerator CoPlayIntro()
    {
        btnGroup.enabled = true;

        float alpha = 0;
        float elapsedTime = 0;

        var logoRect = logoImage.rectTransform;
        var btnRect = btnGroup.transform as RectTransform;

        while(elapsedTime < BtnFadeTime)
        {
            elapsedTime += Time.deltaTime;

            alpha = Mathf.Lerp(1f, 0f, elapsedTime / BtnFadeTime);
            btnGroup.alpha = alpha;
            logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, alpha);

            logoImage.rectTransform.anchoredPosition =
                logoImage.rectTransform.anchoredPosition.x * Vector2.right +
                (logoImage.rectTransform.anchoredPosition.y + moveSpeed * Time.deltaTime) * Vector2.up;

            if(btnRect != null)
                btnRect.anchoredPosition =
                    btnRect.anchoredPosition.y * Vector2.up +
                    (btnRect.anchoredPosition.x - moveSpeed * Time.deltaTime) * Vector2.right;

            yield return null;
        }
    }

    private IEnumerator CoOnVideoEnd()
    {
        yield return wfIntroVideo;
        GameManager.Instance.NewGameStart();
    }
}
