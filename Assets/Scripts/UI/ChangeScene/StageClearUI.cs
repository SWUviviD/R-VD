using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : MonoBehaviour
{
    public Image whiteOverlay; // 하얀 화면
    public Image stageText;
    public Image stageClearText;  // "Clear" 텍스트
    public Button nextButton; // 버튼
    public float fadeSpeed = 1.0f; // 페이드 속도
    public float backgroundAlpha = 0.7f;

    private void Awake()
    {
        // TODO: 다음 스테이지 연결
        //UIHelper.OnClick(nextButton, GameManager.Instance.NextStage);
        UIHelper.OnClick(nextButton, GameManager.Instance.GameRestart);
    }

    private void Start()
    {
        // 텍스트를 처음엔 투명하게 설정
        Color textColor2 = stageText.color;
        textColor2.a = 0;
        stageText.color = textColor2;

        Color textColor1 = stageClearText.color;
        textColor1.a = 0;
        stageClearText.color = textColor1;

        // 버튼 초기 비활성화
        nextButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.IsStageClear)
        {
            HandleBlackOverlayFade();
        }
        else if (whiteOverlay.color.a != 0)
        {
            Color overlayColor = whiteOverlay.color;
            overlayColor.a = 0;
            whiteOverlay.color = overlayColor;

            nextButton.gameObject.SetActive(false);
            Color textColor1 = stageClearText.color;
            textColor1.a = 0;
            stageClearText.color = textColor1;
            Color textColor2 = stageText.color;
            textColor2.a = 0;
            stageText.color = textColor2;
        }
    }

    private void HandleBlackOverlayFade()
    {
        Color overlayColor = whiteOverlay.color;

        if (overlayColor.a < backgroundAlpha)
        {
            overlayColor.a += Time.deltaTime * fadeSpeed;
            whiteOverlay.color = overlayColor;

            if (overlayColor.a >= backgroundAlpha - 0.2f && !nextButton.gameObject.activeSelf)
            {
                nextButton.gameObject.SetActive(true);

                Color textColor1 = stageClearText.color;
                textColor1.a = 1;
                stageClearText.color = textColor1;
                Color textColor2 = stageText.color;
                textColor2.a = 1;
                stageText.color = textColor2;
            }
        }
    }
}
