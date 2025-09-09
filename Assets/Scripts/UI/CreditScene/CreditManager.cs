using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using static Defines.InputDefines;

public class CreditManager : MonoBehaviour
{
    [Header("Scrolling Text")]
    [SerializeField] private RectTransform creditText;  
    [SerializeField] private TextMeshProUGUI creditTMP; 
    [SerializeField] private float scrollSpeed = 50f;    // 올라가는 속도
    [SerializeField] private float waitAfterScroll = 2f; // 스크롤 끝난 뒤 대기 시간

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvas;    
    [SerializeField] private float fadeDuration = 1.5f;  // 페이드 아웃 시간

    private bool isSceneEnding = false;
    private float screenHeight;

    private void Start()
    {
        Canvas canvas = creditText.GetComponentInParent<Canvas>();
        screenHeight = canvas != null ? canvas.GetComponent<RectTransform>().rect.height : Screen.height;

        if (fadeCanvas != null)
            fadeCanvas.alpha = 0f;
    }

    private void Update()
    {
        if (creditText == null || creditTMP == null || isSceneEnding)
            return;

        creditText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        float textBottomY = creditText.anchoredPosition.y - creditTMP.preferredHeight * creditText.pivot.y;

        if (textBottomY >= screenHeight)
        {
            StartCoroutine(WaitAndLoadScene());
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(WaitAndLoadScene());
    }

    private void OnSkipCredits(InputAction.CallbackContext ctx)
    {
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        if (isSceneEnding) yield break;
        isSceneEnding = true;

        yield return new WaitForSeconds(waitAfterScroll);

        if (fadeCanvas != null)
            yield return FadeCanvas(1f, fadeDuration);

        SceneManager.LoadScene("TitleScene");
    }

    private IEnumerator FadeCanvas(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvas.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}
