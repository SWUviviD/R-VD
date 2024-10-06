using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraSkill : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform target;

    [SerializeField] private float zoomOutMaxTime = 1.0f; // 최대시간
    [SerializeField] private float maxZoomOutDistance = 10.0f; // 최장거리
    [SerializeField] private float zoomInReturnTime = 0.2f; // 돌아가는 시간

    private float currentZoomTime = 0.0f;
    private bool isZoomingOut = false;
    private Vector3 initialFollowOffset; // 카메라 초기 오프셋

    private CinemachineTransposer transposer; // 카메라 transposer 참조

    /// <summary>
    /// 초기화 확인
    /// <summary>
    private void Start()
    {
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                // 카메라 초기 오프셋 설정
                initialFollowOffset = new Vector3(transposer.m_FollowOffset.x, 6.0f,
                    transposer.m_FollowOffset.z);
                transposer.m_FollowOffset = initialFollowOffset;
            }
        }
    }

    private void Update()
    {
        /// <summary>
        /// 스킬1(Q) 버튼 Down/Up 확인
        /// <summary>
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

    /// <summary>
    /// 카메라 줌아웃 (MaxTime만큼 줌아웃)
    /// <summary>
    private void ZoomOutCamera()
    {
        if (transposer == null || target == null)
            return;

        currentZoomTime += Time.deltaTime;
        if (currentZoomTime > zoomOutMaxTime)
        {
            currentZoomTime = zoomOutMaxTime;
        }

        // 초기 오프셋과 최대 줌아웃 사이에서 카메라 오프셋 계산
        float newOffsetZ = Mathf.Lerp(initialFollowOffset.z,
            initialFollowOffset.z - maxZoomOutDistance, currentZoomTime / zoomOutMaxTime);
        // 계산값을 카메라 Transposer에 적용
        transposer.m_FollowOffset = new Vector3(initialFollowOffset.x, 6.0f, newOffsetZ);
    }

    /// <summary>
    /// 카메라 원위치 (줌아웃 취소)
    /// <summary>
    private IEnumerator ResetCamera()
    {
        float elapsedTime = 0.0f;
        Vector3 startOffset = transposer.m_FollowOffset;

        // 원위치 시간동안 카메라 오프셋 복구
        while (elapsedTime < zoomInReturnTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / zoomInReturnTime;
            // 카메라 오프셋을 원래대로 천천히 복구
            float newOffsetZ = Mathf.Lerp(startOffset.z, initialFollowOffset.z, t);
            transposer.m_FollowOffset = new Vector3(initialFollowOffset.x, 6.0f, newOffsetZ);
            yield return null; // 프레임 대기
        }

        // 초기 오프셋으로 마지막 최종 복구
        transposer.m_FollowOffset = initialFollowOffset;
        currentZoomTime = 0.0f;
    }
}
