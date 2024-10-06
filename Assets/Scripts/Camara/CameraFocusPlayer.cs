using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraFocusPlayer : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// 기본 거리 세팅
    /// <summary>
    [Header("Camera Distances")]
    public float defaultDistance = 10f; // 기본 거리
    public float shiftDistance = 10.5f; // 대쉬 중 거리
    public float shiftAndSpaceDistance = 11f; // 대쉬+점프 저리
    public float zoomInDistance = 2f; // 줌인 거리

    /// <summary>
    /// 카메라 전환, 속도 세팅
    /// <summary>
    [Header("Transition Settings")]
    public float transitionSpeed = 5f; // 거리 전환 속도
    public float zoomInSpeed = 0.5f; // 줌인 속도

    private CinemachineTransposer transposer; // 카메라 객체
    private float targetDistance;
    private bool isShiftPressed = false;
    private bool isSpacePressed = false;

    [Header("Player Settings")]
    public PlayerStatus playerStatus; // 플레이어 상태 확인

    [SerializeField] private float baseDistance = 25f; // 플레이어와의 기본 거리
    public float BaseDistance => baseDistance;

    private Vector3 previousPlayerPosition; // 이전 프레임의 플레이어 위치
    private float playerMovementThreshold = 0.1f; // 플레이어가 멈췄는지 판단하는 값

    private float timeSincePlayerStopped = 0f; // 플레이어 멈춘 후 경과 시간
    private float zoomInDelay = 0.3f; // 줌인 전 대기 시간

    /// <summary>
    /// 초기화 확인
    /// <summary>
    private void Start()
    {
        if (virtualCamera == null)
        {
            LogManager.LogError("Cinemachine Virtual Camera가 할당되지 않았습니다.");
            return;
        }

        if (playerStatus == null)
        {
            LogManager.LogError("PlayerStatus가 할당되지 않았습니다.");
            return;
        }

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            LogManager.LogError("Cinemachine Transposer를 찾을 수 없습니다.");
            return;
        }

        /// <summary>
        /// 기본 카메라 거리 설정
        /// <summary>
        targetDistance = defaultDistance;
        Vector3 currentOffset = transposer.m_FollowOffset;
        currentOffset.z = -defaultDistance;
        transposer.m_FollowOffset = currentOffset;

        /// <summary>
        /// 플레이어 초기 위치 저장
        /// <summary>
        previousPlayerPosition = playerStatus.transform.position;
    }

    private void Update()
    {
        HandleCameraDistance();
        HandleZoomInOnPlayerStop();
    }

    /// <summary>
    /// 카메라 거리 관리 함수
    /// 플레이어의 대쉬/점프에 따라 거리 변경
    /// <summary>
    private void HandleCameraDistance()
    {
        if (isShiftPressed && isSpacePressed)
        {
            targetDistance = shiftAndSpaceDistance;
        }
        else if (isShiftPressed)
        {
            targetDistance = shiftDistance;
        }
        else
        {
            targetDistance = defaultDistance;
        }

        if (playerStatus != null && playerStatus.IsDashing)
        {
            targetDistance = shiftAndSpaceDistance;
        }

        /// <summary>
        /// 카메라 거리 전환 스무딩
        /// <summary>
        Vector3 currentOffset = transposer.m_FollowOffset;
        currentOffset.z = Mathf.Lerp(currentOffset.z, -targetDistance, Time.deltaTime * transitionSpeed);
        transposer.m_FollowOffset = currentOffset;
    }

    /// <summary>
    /// 플레이어가 멈췄을 때 줌인하는 함수
    /// 플레이어가 멈추면 zoomInDelay만큼 대기 후 zoomInDistance만큼 줌인
    /// <summary>
    private void HandleZoomInOnPlayerStop()
    {
        float playerSpeed = (playerStatus.transform.position - previousPlayerPosition).magnitude / Time.deltaTime;

        if (playerSpeed < playerMovementThreshold) // 플레이어가 멈추면
        {
            timeSincePlayerStopped += Time.deltaTime;

            if (timeSincePlayerStopped >= zoomInDelay) // 딜레이 시간만큼 대기 후 줌인
            {
                Vector3 currentOffset = transposer.m_FollowOffset;
                currentOffset.z = Mathf.Lerp(currentOffset.z, -zoomInDistance, Time.deltaTime * zoomInSpeed);
                transposer.m_FollowOffset = currentOffset;
            }
        }
        else
        {
            timeSincePlayerStopped = 0f;
        }

        previousPlayerPosition = playerStatus.transform.position; // 이전 프레임 플레이어 위치 갱신
    }

    /// <summary>
    /// 대쉬, 점프 이벤트 핸들러
    /// <summary>
    private void OnShiftPressed(InputAction.CallbackContext context)
    {
        isShiftPressed = true;
    }

    private void OnShiftReleased(InputAction.CallbackContext context)
    {
        isShiftPressed = false;
    }

    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        isSpacePressed = true;
    }

    private void OnSpaceReleased(InputAction.CallbackContext context)
    {
        isSpacePressed = false;
    }
}