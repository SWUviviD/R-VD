using UnityEngine;
using UnityEngine.UI;

public class ScrollbarFade : MonoBehaviour
{
    public ScrollRect scrollRect;
    public CanvasGroup canvasGroup;

    public float fadeDelay = 0.5f;
    public float fadeTime = 0.5f;

    // [추가] 미세한 움직임을 무시할 임계값
    public float threshold = 0.0005f;

    private float targetAlpha = 0f;
    private float lastScrollTime;
    private Vector2 lastPosition;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        lastPosition = scrollRect.normalizedPosition;
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    void OnScrollChanged(Vector2 currentPos)
    {
        // 핵심: 이전 위치와 현재 위치의 차이가 임계값보다 클 때만 타이머 갱신
        if (Vector2.Distance(lastPosition, currentPos) > threshold)
        {
            targetAlpha = 1f;
            lastScrollTime = Time.time;
            lastPosition = currentPos;
        }
    }

    void Update()
    {
        // 알파값 전환 (나타날 땐 0.1초, 사라질 땐 fadeTime 설정값대로)
        float speed = (targetAlpha > 0) ? 0.1f : fadeTime;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime / speed);

        // 일정 시간 동안 "의미 있는" 움직임이 없으면 사라지기 시작
        if (targetAlpha > 0 && Time.time - lastScrollTime > fadeDelay)
        {
            targetAlpha = 0f;
        }
    }
}