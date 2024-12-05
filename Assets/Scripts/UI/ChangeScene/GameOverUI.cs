using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Image blackOverlay; // 검은 화면
    public Text gameOverText;  // "Game Over" 텍스트
    public Button restartButton, exitButton; // 버튼
    public float fadeSpeed = 1.0f; // 페이드 속도

    public bool isGameOver = false;

    private void Start()
    {
        // 텍스트를 처음엔 투명하게 설정
        Color textColor = gameOverText.color;
        textColor.a = 0;
        gameOverText.color = textColor;

        // 버튼 초기 비활성화
        restartButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

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

                Color textColor = gameOverText.color;
                textColor.a = 1;
                gameOverText.color = textColor;
            }
        }
    }

    public void RestartGame()
    {
        // 재시작 처리
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        // 게임 종료
        Application.Quit();
    }
}
