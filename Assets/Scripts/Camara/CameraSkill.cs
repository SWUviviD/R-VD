using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraSkill : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform target;
    [SerializeField] private float zoomOutMaxTime = 1.0f; // 최대시간
    [SerializeField] private float maxZoomOutDistance = 10.0f; // 최장거리
    [SerializeField] private float zoomInReturnTime = 0.5f; // 돌아가는 시간

    private float currentZoomTime = 0.0f;
    private bool isZoomingOut = false;
    private Vector3 initialFollowOffset;

    private CinemachineTransposer transposer;

    private void Start()
    {
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                initialFollowOffset = new Vector3(transposer.m_FollowOffset.x, 6.0f, transposer.m_FollowOffset.z);
                transposer.m_FollowOffset = initialFollowOffset;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (!isZoomingOut)
            {
                isZoomingOut = true;
                StopAllCoroutines();
            }
            ZoomOutCamera();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            isZoomingOut = false;
            StartCoroutine(ResetCamera());
        }
    }

    private void ZoomOutCamera()
    {
        if (transposer == null || target == null)
            return;

        currentZoomTime += Time.deltaTime;
        if (currentZoomTime > zoomOutMaxTime)
        {
            currentZoomTime = zoomOutMaxTime;
        }

        float newOffsetZ = Mathf.Lerp(initialFollowOffset.z, initialFollowOffset.z - maxZoomOutDistance, currentZoomTime / zoomOutMaxTime);
        transposer.m_FollowOffset = new Vector3(initialFollowOffset.x, 6.0f, newOffsetZ);
    }

    private IEnumerator ResetCamera()
    {
        float elapsedTime = 0.0f;
        Vector3 startOffset = transposer.m_FollowOffset;

        while (elapsedTime < zoomInReturnTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / zoomInReturnTime;
            float newOffsetZ = Mathf.Lerp(startOffset.z, initialFollowOffset.z, t);
            transposer.m_FollowOffset = new Vector3(initialFollowOffset.x, 6.0f, newOffsetZ);
            yield return null;
        }

        transposer.m_FollowOffset = initialFollowOffset;
        currentZoomTime = 0.0f;
    }
}
