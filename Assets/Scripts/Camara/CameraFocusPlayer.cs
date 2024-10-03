using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraFocusPlayer : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("Camera Distances")]
    public float defaultDistance = 10f;
    public float shiftDistance = 12f;
    public float shiftAndSpaceDistance = 15f;
    public float zoomInDistance = 3f;

    [Header("Transition Settings")]
    public float transitionSpeed = 5f;
    public float zoomInSpeed = 0.2f;

    private CinemachineTransposer transposer;
    private float targetDistance;
    private bool isShiftPressed = false;
    private bool isSpacePressed = false;

    [Header("Player Settings")]
    public PlayerStatus playerStatus;

    [SerializeField] private float baseDistance = 25f;
    public float BaseDistance => baseDistance;

    private Vector3 previousPlayerPosition;
    private float playerMovementThreshold = 0.1f;

    private float timeSincePlayerStopped = 0f;
    private float zoomInDelay = 1f;

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

        targetDistance = defaultDistance;
        Vector3 currentOffset = transposer.m_FollowOffset;
        currentOffset.z = -defaultDistance;
        transposer.m_FollowOffset = currentOffset;

        previousPlayerPosition = playerStatus.transform.position;
    }

    private void FixedUpdate()
    {
        HandleCameraDistance();
        HandleZoomInOnPlayerStop();
    }

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

        Vector3 currentOffset = transposer.m_FollowOffset;
        currentOffset.z = Mathf.Lerp(currentOffset.z, -targetDistance, Time.deltaTime * transitionSpeed);
        transposer.m_FollowOffset = currentOffset;
    }

    private void HandleZoomInOnPlayerStop()
    {
        float playerSpeed = (playerStatus.transform.position - previousPlayerPosition).magnitude / Time.deltaTime;

        if (playerSpeed < playerMovementThreshold)
        {
            timeSincePlayerStopped += Time.deltaTime;

            if (timeSincePlayerStopped >= zoomInDelay)
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

        previousPlayerPosition = playerStatus.transform.position;
    }

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