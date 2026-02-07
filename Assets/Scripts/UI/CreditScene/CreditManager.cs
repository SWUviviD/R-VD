using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using static Defines.InputDefines;

public class CreditManager : MonoBehaviour
{
    [Header("Scrolling Text")]
    [SerializeField] private RectTransform creditPanel;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float scrollSpeed = 50f;    // 올라가는 속도
    [SerializeField] private float waitAfterScroll = 2f; // 스크롤 끝난 뒤 대기 시간

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvas;    
    [SerializeField] private float fadeDuration = 1.5f;  // 페이드 아웃 시간

    private bool canScroll = false;
    private bool isSceneEnding = false;
    private float screenHeight;

    private void Start()
    {
        screenHeight = canvas != null
            ? canvas.GetComponent<RectTransform>().rect.height
            : Screen.height;

        if (fadeCanvas != null)
            fadeCanvas.alpha = 1f;

        if (creditPanel == null)
        {
            enabled = false;
            return;
        }

        StartCoroutine(FadeInAndStartScroll());
    }

    private void Update()
    {
        if (canScroll == false || isSceneEnding) return;

        creditPanel.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditPanel.anchoredPosition.y - creditPanel.rect.height > screenHeight)
        {
            StartCoroutine(WaitAndLoadScene());
        }

        if (Mouse.current.leftButton.wasPressedThisFrame ||
            Input.GetKeyDown(KeyCode.Space))        
        {
            StartCoroutine(WaitAndLoadScene());
        }
    }

    private void OnSkipCredits(InputAction.CallbackContext ctx)
    {
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator FadeInAndStartScroll()
    {
        if (fadeCanvas != null)
            yield return FadeCanvas(0f, fadeDuration);

        canScroll = true;
    }

    private IEnumerator WaitAndLoadScene()
    {
        if (isSceneEnding) yield break;
        isSceneEnding = true;

        yield return new WaitForSeconds(waitAfterScroll);

        if (fadeCanvas != null)
            yield return FadeCanvas(1f, fadeDuration);

        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Title);
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
