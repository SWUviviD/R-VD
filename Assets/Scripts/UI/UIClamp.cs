using UnityEngine;

public class UIClamp : MonoBehaviour
{
    /// <summary> 위치가 보정되는 UI의 최상위 부모 </summary>
    [SerializeField] private Canvas clampCanvas;
    /// <summary> UI가 따라가는 target의 Transform </summary>
    [SerializeField] private Transform targetTR;
    /// <summary> 보정 길이: UI가 Offset만큼 벗어나면 사라진다. </summary>
    [SerializeField] private Vector2 offset;

    private RectTransform rectTR;
    private float rectWidth;
    private float rectHeight;

    private Vector2 targetPoint;
    private float rectLeft;
    private float rectRight;
    private float rectBottom;
    private float rectTop;

    private void Awake()
    {
        transform.parent = clampCanvas.transform;

        rectTR = GetComponent<RectTransform>();
        rectWidth = rectTR.rect.width;
        rectHeight = rectTR.rect.height;
    }

    private void LateUpdate()
    {
        // Target Position을 Screen Position으로 변환
        targetPoint = Camera.main.WorldToScreenPoint(targetTR.position);
        rectLeft = targetPoint.x - rectWidth / 2;
        rectRight = targetPoint.x + rectWidth / 2;
        rectBottom = targetPoint.y - rectHeight / 2;
        rectTop = targetPoint.y + rectHeight / 2;

        // Left 보정
        if (rectLeft < -offset.x)
        {
            targetPoint.x += offset.x;
        }
        else if (rectLeft < 0)
        {
            targetPoint.x = rectWidth / 2;
        }
        // Right 보정
        else if (rectRight > Camera.main.pixelWidth + offset.x)
        {
            targetPoint.x -= offset.x;
        }
        else if (rectRight > Camera.main.pixelWidth)
        {
            targetPoint.x = Camera.main.pixelWidth - rectWidth / 2;
        }

        // Bottom 보정
        if (rectBottom < -offset.y)
        {
            targetPoint.y += offset.y;
        }
        else if (rectBottom < 0)
        {
            targetPoint.y = rectHeight / 2;
        }
        // Top 보정
        else if (rectTop > Camera.main.pixelHeight + offset.y)
        {
            targetPoint.y -= offset.y;
        }
        else if (rectTop > Camera.main.pixelHeight)
        {
            targetPoint.y = Camera.main.pixelHeight - rectHeight / 2;
        }

        // 위치 갱신
        rectTR.position = targetPoint;
    }
}
