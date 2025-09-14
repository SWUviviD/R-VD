using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public Image blackOverlay; // 검은 화면
    public Image stageText;
    public TextMeshProUGUI gameOverText;  // "Game Over" 텍스트
    public Button restartButton, exitButton; // 버튼
    public float fadeSpeed = 1.0f; // 페이드 속도

    private void Awake()
    {
        UIHelper.OnClick(restartButton, GameManager.Instance.GameRestart);
        UIHelper.OnClick(exitButton, Exit);
    }

    private void Exit()
    {
        GameManager.Instance.ResetStageGameData();
        GameManager.Instance.GameExit(false);
    }


    private void Start()
    {
        // 텍스트를 처음엔 투명하게 설정
        Color textColor2 = stageText.color;
        textColor2.a = 0;
        stageText.color = textColor2;

        Color textColor1 = gameOverText.color;
        textColor1.a = 0;
        gameOverText.color = textColor1;

        // 버튼 초기 비활성화
        restartButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            HandleBlackOverlayFade();
        }
        else if (blackOverlay.color.a != 0)
        {
            Color overlayColor = blackOverlay.color;
            overlayColor.a = 0;
            blackOverlay.color = overlayColor;

            restartButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);

            Color textColor1 = gameOverText.color;
            textColor1.a = 0;
            gameOverText.color = textColor1;
            Color textColor2 = stageText.color;
            textColor2.a = 0;
            stageText.color = textColor2;
        }
    }

    private void HandleBlackOverlayFade()
    {
        Color overlayColor = blackOverlay.color;

        if (overlayColor.a < 0.9f)
        {
            overlayColor.a += Time.deltaTime * fadeSpeed;
            blackOverlay.color = overlayColor;

            if (overlayColor.a >= 0.7f && !restartButton.gameObject.activeSelf)
            {
                restartButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(true);

                Color textColor1 = gameOverText.color;
                textColor1.a = 1;
                gameOverText.color = textColor1;
                Color textColor2 = stageText.color;
                textColor2.a = 1;
                stageText.color = textColor2;
            }
        }
    }
}
