using Defines;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 focusOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Orbit")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float height = 2f;            // 기본 높이 바이어스(피치와 별개로 항상 위로 띄움)

    [SerializeField] private float yawSensitivity = 0.15f; // 좌우 감도
    [SerializeField] private float pitchSensitivity = 0.12f; // 상하 감도
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    [Tooltip("피치(상하) 각도 제한")]
    [SerializeField] private float minPitch = -30f;        // 아래로
    [SerializeField] private float maxPitch = 70f;         // 위로

    [Header("Zoom")]
    [SerializeField] private float zoomSensitivity = 0.5f;

    [Header("Collision (Raycast)")]
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float collisionBuffer = 0.10f;
    [SerializeField] private float minCollisionDistance = 1.0f;

    [Header("Smoothing")]
    [Tooltip("체크하면 멀미 최소화를 위해 스냅(보간 없음). 해제 시 아래 댐핑 값 사용")]
    [SerializeField] private bool useMinimalDamping = true;
    [SerializeField] private float positionDamping = 12f; // 해제 시 위치 보간 세기
    [SerializeField] private float rotationDamping = 20f; // 해제 시 회전 보간 세기

    private float _yaw;      // 누적 Yaw
    private float _pitch;    // 누적 Pitch

    void OnValidate()
    {
        distance = Mathf.Max(distance, 0.01f);
        minCollisionDistance = Mathf.Clamp(minCollisionDistance, 0.05f, distance);
        maxPitch = Mathf.Max(maxPitch, minPitch + 1f);
    }

    void Start()
    {
        if (!target)
        {
            Debug.LogError("[OrbitCamera] Target이 비어 있습니다.");
            enabled = false;
            return;
        }

        // 초기 yaw, pitch 계산
        Vector3 focus = target.position + focusOffset;
        Vector3 toCam = transform.position - (focus + Vector3.up * 0f);
        if (toCam.sqrMagnitude < 1e-6f) toCam = new Vector3(0f, height, -distance);

        // yaw
        Vector3 flat = toCam; flat.y = 0f;
        if (flat.sqrMagnitude < 1e-6f) flat = Vector3.back;
        _yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;

        // pitch
        float flatMag = flat.magnitude;
        _pitch = Mathf.Atan2(toCam.y, flatMag) * Mathf.Rad2Deg;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        ConnectInputToFunc();
    }

    private void ConnectInputToFunc()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Camera),
            InputDefines.ActionPoint.IsPerformed, OnLookInput);
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Camera),
            InputDefines.ActionPoint.IsPerformed, OnLookInput);

        //InputManager.Instance.RemoveInputEventFunction(
        //    new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraZoom),
        //    InputDefines.ActionPoint.IsPerformed, OnZoomInput);
        //InputManager.Instance.AddInputEventFunction(
        //    new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraZoom),
        //    InputDefines.ActionPoint.IsPerformed, OnZoomInput);
    }

    private void OnLookInput(InputAction.CallbackContext ctx)
    {
        if (!enabled) return;

        Vector2 look = ctx.action.ReadValue<Vector2>();
        float dx = look.x * (invertX ? -1f : 1f);
        float dy = look.y * (invertY ? 1f : -1f); // 일반적으로 마우스 위=카메라 아래로, 그래서 기본 -1

        _yaw += dx * yawSensitivity;
        _pitch += dy * pitchSensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        // Yaw는 0~360 래핑
        if (_yaw > 360f || _yaw < -360f) _yaw = Mathf.Repeat(_yaw, 360f);
    }

    private void OnZoomInput(InputAction.CallbackContext ctx)
    {
        if (!enabled) return;

        Vector2 delta = ctx.action.ReadValue<Vector2>();
        distance = Mathf.Clamp(distance + delta.y * zoomSensitivity, minDistance, maxDistance);
    }

    void OnEnable() => GameManager.Instance.SetCameraInput(true);
    void OnDisable() => GameManager.Instance.SetCameraInput(false);

    private void OnDestroy()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Camera),
            InputDefines.ActionPoint.IsPerformed, OnLookInput);

        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraZoom),
            InputDefines.ActionPoint.IsPerformed, OnZoomInput);
    }

    void LateUpdate()
    {
        if (!enabled || !target) return;

        Vector3 focus = target.position + focusOffset;

        // 피치/야우로 공전 (height는 상시 바이어스)
        Quaternion orbitRot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 baseOffset = new Vector3(0f, height, -distance);
        Vector3 desiredPos = focus + orbitRot * baseOffset;

        // 충돌 보정
        Vector3 toCam = desiredPos - focus;
        float desiredDist = toCam.magnitude;
        Vector3 dir = toCam.sqrMagnitude > 1e-6f ? toCam.normalized : Vector3.back;

        float adjustedDist = desiredDist;
        if (Physics.Raycast(focus, dir, out RaycastHit hit, desiredDist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            adjustedDist = Mathf.Max(hit.distance - collisionBuffer, minCollisionDistance);
        }
        Vector3 finalPos = focus + dir * adjustedDist;

        // 멀미 최소: 스냅(보간 없음). 필요 시 해제하고 보간 사용
        if (useMinimalDamping)
        {
            transform.position = finalPos;
            transform.rotation = Quaternion.LookRotation(focus - finalPos, Vector3.up);
            return;
        }

        // 부드러운 전환(옵션)
        float tPos = 1f - Mathf.Exp(-positionDamping * Time.unscaledDeltaTime);
        float tRot = 1f - Mathf.Exp(-rotationDamping * Time.unscaledDeltaTime);
        transform.position = Vector3.Lerp(transform.position, finalPos, tPos);
        Quaternion lookRot = Quaternion.LookRotation(focus - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, tRot);
    }

    /// <summary>
    /// 플레이어 기준 특정 각도로 즉시 회전(컷신/리셋용)
    /// </summary>
    public void SetCameraRotation(float relativeAngleDeg)
    {
        if (!target) return;

        float playerYaw = target.eulerAngles.y;
        _yaw = Mathf.Repeat(playerYaw + relativeAngleDeg, 360f);

        // 위치 즉시 반영
        Vector3 focus = target.position + focusOffset;
        Quaternion orbitRot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 baseOffset = new Vector3(0f, height, -distance);
        Vector3 desiredPos = focus + orbitRot * baseOffset;

        Vector3 toCam = desiredPos - focus;
        float desiredDist = toCam.magnitude;
        Vector3 dir = toCam.normalized;

        float adjustedDist = desiredDist;
        if (Physics.Raycast(focus, dir, out RaycastHit hit, desiredDist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            adjustedDist = Mathf.Max(hit.distance - collisionBuffer, minCollisionDistance);
        }
        Vector3 finalPos = focus + dir * adjustedDist;

        transform.position = finalPos;
        transform.rotation = Quaternion.LookRotation(focus - finalPos, Vector3.up);
    }
}
