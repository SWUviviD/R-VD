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

    [Header("Transition Settings")]
    public float transitionSpeed = 5f;    

    private CinemachineTransposer transposer;
    private float targetDistance;
    private bool isShiftPressed = false;
    private bool isSpacePressed = false;

    [Header("Player Settings")]
    public PlayerStatus playerStatus;

    [SerializeField] private float baseDistance = 25f;
    public float BaseDistance => baseDistance;


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

        // 기본 거리 설정
        targetDistance = defaultDistance;
        Vector3 currentOffset = transposer.m_FollowOffset;
        currentOffset.z = -defaultDistance; 
        transposer.m_FollowOffset = currentOffset;
    }

    private void OnEnable()
    {
        InputManager.Instance.AddInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Dash),
            Defines.InputDefines.ActionPoint.IsStarted,
            OnShiftPressed);

        InputManager.Instance.AddInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Dash),
            Defines.InputDefines.ActionPoint.IsCanceled,
            OnShiftReleased);

        InputManager.Instance.AddInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Jump),
            Defines.InputDefines.ActionPoint.IsStarted,
            OnSpacePressed);

        InputManager.Instance.AddInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Jump),
            Defines.InputDefines.ActionPoint.IsCanceled,
            OnSpaceReleased);
    }

    private void OnDisable()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Dash),
            Defines.InputDefines.ActionPoint.IsStarted,
            OnShiftPressed);

        InputManager.Instance.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Dash),
            Defines.InputDefines.ActionPoint.IsCanceled,
            OnShiftReleased);

        InputManager.Instance.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Jump),
            Defines.InputDefines.ActionPoint.IsStarted,
            OnSpacePressed);

        InputManager.Instance.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Jump),
            Defines.InputDefines.ActionPoint.IsCanceled,
            OnSpaceReleased);
    }

    private void FixedUpdate()
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
