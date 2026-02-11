using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using static Defines.InputDefines;

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
    [SerializeField] private Image bgImage;

    [Header("Btn")]
    [SerializeField] private CanvasGroup btnGroup;
    [SerializeField] private Button StartBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button CreditBtn;
    [SerializeField] private Button SettingBtn;
    [SerializeField] private Button ExitBtn;
    [SerializeField] private Button HowToBtn;
    [SerializeField] private Image HowToBtnImage;
    [SerializeField] private Color btnEnableTextColor;
    [SerializeField] private Color btnEnableImageColor;
    [SerializeField] private GameObject disableBtnClick;

    [Header("Setting")]
    [SerializeField] private TitleSettingUI settingUI;
    [SerializeField] private Image skipRoll;
    [SerializeField] private Image skipRollbg;
    [SerializeField] private float skipTime = 2f;

    private bool isShowingIntro = false;

    private void Start()
    {
        SoundManager.Instance.StopBGM();

        btnGroup.enabled = false;

        //bgPlayer.gameObject.SetActive(false);
        //bgPlayer.isLooping = true;
        //bgPlayer.playOnAwake = false;

        GameManager.Instance.ShowCursor();

        wfIntroVideo = new WaitForSeconds(introTime);

        UIHelper.OnClick(StartBtn, NewGameStart);
        UIHelper.OnClick(ExitBtn, GameExit);
        UIHelper.OnClick(CreditBtn, LoadCredit);

        if (GameManager.Instance.GameDataManager.LoadGameData() == true)
        {
            ContinueBtn.GetComponentInChildren<Text>().raycastTarget = true;
            ContinueBtn.GetComponentInChildren<Text>().color = btnEnableTextColor;
            ContinueBtn.GetComponent<Image>().color = btnEnableImageColor;
            ContinueBtn.GetComponent<SettingButtonHoverEffect>().enabled = true;
            UIHelper.OnClick(ContinueBtn, GameManager.Instance.LoadData);
        }

        UIHelper.OnClick(SettingBtn, () => settingUI.ShowPanel(true));

        UIHelper.OnClick(HowToBtn, HowToUI.Instance.ShowHowToUI);

        skipRollbg.gameObject.SetActive(false);
    }

    private float elapsedTime = 0f;
    private bool isPressing = false;
    private void OnKeyPressed(InputAction.CallbackContext context)
    {
        elapsedTime = 0f;
        skipRoll.fillAmount = 0f;
        skipRollbg.gameObject.SetActive(true);
        isPressing = true;
    }

    private void OnKeyCanceled(InputAction.CallbackContext context)
    {
        elapsedTime = 0f;
        skipRollbg.gameObject.SetActive(false);
        isPressing = false;
    }

    private void Update()
    {
        if(isShowingIntro == true && isPressing == true)
        {
            elapsedTime += Time.deltaTime;
            skipRoll.fillAmount = elapsedTime / skipTime;
            if(elapsedTime > skipTime)
            {
                StopAllCoroutines();

                InputManager.Instance.RemoveInputEventFunction(
                    new InputActionName(ActionMapType.PlayerActions, "UINext"),
                    ActionPoint.IsStarted, OnKeyPressed);

                InputManager.Instance.RemoveInputEventFunction(
                    new InputActionName(ActionMapType.PlayerActions, "UINext"),
                    ActionPoint.IsCanceled, OnKeyCanceled);
                
                GameManager.Instance.NewGameStart();
            }
        }
    }

    private void NewGameStart()
    {
        //CutSceneManager.Instance.PlayCutScene(
        //    Defines.CutSceneDefines.CutSceneNumber.Intro,
        //    GameManager.Instance.NewGameStart);

        // 컷씬 말고 영상으로 대체
        bgPlayer.gameObject.SetActive(true);
        //bgPlayer.clip = IntroVideo;
        //bgPlayer.isLooping = false;
        //bgPlayer.Play();

        isShowingIntro = true;

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsStarted, OnKeyPressed);

        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsCanceled, OnKeyCanceled);

        disableBtnClick.gameObject.SetActive(true);

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
            HowToBtnImage.color = new Color(HowToBtnImage.color.r, HowToBtnImage.color.g, HowToBtnImage.color.b, alpha);
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, alpha);

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

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsStarted, OnKeyPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, "UINext"),
            ActionPoint.IsCanceled, OnKeyCanceled);

        GameManager.Instance.NewGameStart();
    }
}
