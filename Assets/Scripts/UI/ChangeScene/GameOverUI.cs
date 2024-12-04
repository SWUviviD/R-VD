using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Image blackOverlay; // 검은 화면
    public Text gameOverText;  // "Game Over" 텍스트
    public Button restartButton, exitButton; // 버튼
    public float fadeSpeed = 1.0f; // 페이드 속도

    private bool isGameOver = false;

    private void Update()
    {
        if (isGameOver)
        {
            HandleBlackOverlayFade();
        }
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, 1);
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
            }
        }
    }

    public void RestartGame()
    {
        // 여기에 재시작 함수
    }

    public void ExitGame()
    {
        // 여기에 종료 함수
    }
}
