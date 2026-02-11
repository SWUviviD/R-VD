using UnityEngine;

public class ResolutionFixer : MonoBehaviour
{
    [SerializeField] private float targetWidth;
    [SerializeField] private float targetHeight;

    private void Awake()
    {
        float targetAspectRatio = targetWidth / targetHeight;

        float currentAspectRatio = (float)Screen.width / Screen.height;

        float scaleHeight = currentAspectRatio / targetAspectRatio;

        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}