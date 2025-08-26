using Defines;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;                       // 따라갈 대상(플레이어)
    private Vector3 focusOffset = new Vector3(0f, 1.6f, 0f); // 시선 중심(플레이어 머리 높이 등)

    [Header("Orbit (Yaw only)")]
    [SerializeField] private float distance = 6f;                    // 기본 거리
    [SerializeField] private float maxDistance = 20f;               // 최대 거리
    [SerializeField] private float minDistance = 4f;                // 최소 거리
    [SerializeField] private float height = 2f;                      // 카메라 높이(타깃 기준)

    [SerializeField] private float yawSensitivity = 0.15f;           // 마우스/스틱 입력 스케일 (픽셀/유닛당 각도)
    [SerializeField] private float zoomSensitivity = 0.5f;           // 줌임 줌아웃 
    [SerializeField] private bool invertX = false;                   // 좌우 반전

    [Header("Collision (Raycast)")]
    [SerializeField] private LayerMask collisionMask = ~0;           // 지형/벽 레이어
    [SerializeField] private float collisionBuffer = 0.10f;          // 벽에서 띄울 거리
    [SerializeField] private float minCollisionDistance = 1.0f;               // 너무 가까워지지 않도록 제한

    [Header("Smoothing")]
    [SerializeField] private float positionDamping = 12f;            // 위치 보간 세기
    [SerializeField] private float rotationDamping = 20f;            // 회전 보간 세기

    private float _yaw;            // 누적된 Yaw 각도(도)
    private Vector3 _vel;          // SmoothDamp용

    private bool _enable = false;

    void OnValidate()
    {
        distance = Mathf.Max(distance, 0.01f);
        minCollisionDistance = Mathf.Clamp(minCollisionDistance, 0.05f, distance);
    }

    void Start()
    {
        if (!target)
        {
            Debug.LogError("[OrbitYawOnly] Target이 비어 있습니다.");
            enabled = false;
            return;
        }

        // 현재 카메라 기준으로 초기 Yaw 계산
        Vector3 focus = target.position + focusOffset;
        Vector3 dir = transform.position - (focus + Vector3.up * 0f);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.back; // 동일 위치 방지
        _yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ConnectInputToFunc();
    }

    private void ConnectInputToFunc()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Camera),
            InputDefines.ActionPoint.IsPerformed, OnMouseInput);
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Camera),
            InputDefines.ActionPoint.IsPerformed, OnMouseInput);

        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraZoom),
            InputDefines.ActionPoint.IsPerformed, OnScroolInput);
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.CameraZoom),
            InputDefines.ActionPoint.IsPerformed, OnScroolInput);
    }

    private void OnMouseInput(InputAction.CallbackContext _playerStatus)
    {
        if (enabled == false)
            return;

        // Vector2(x: 좌우, y: 상하) — 여기선 좌우(Yaw)만 사용
        Vector2 look = _playerStatus.action.ReadValue<Vector2>();
        float deltaX = look.x * (invertX ? -1f : 1f);
        _yaw += deltaX * yawSensitivity;      // 마우스 델타 기준이면 deltaTime 곱하지 않는 편이 자연스러움
        // Yaw는 제한 없음(0~360 래핑 자동)
    }

    private void OnScroolInput(InputAction.CallbackContext _playerStatus)
    {
        if (enabled == false)
            return;

        Vector2 delta = _playerStatus.action.ReadValue<Vector2>();
        distance = Mathf.Clamp(distance + delta.y * zoomSensitivity, minDistance, maxDistance);
    }

    void OnEnable()
    {
        GameManager.Instance.SetCameraInput(true);
    }

    void OnDisable()
    {
        GameManager.Instance.SetCameraInput(false);
    }

    void LateUpdate()
    {
        if (enabled == false) return;
        if (!target) return;

        Vector3 focus = target.position + focusOffset;

        // 원하는(충돌 전) 카메라 위치: Yaw 회전으로 수평 공전 + 고정 높이/거리
        Quaternion yawRot = Quaternion.Euler(0f, _yaw, 0f);
        Vector3 localOffset = new Vector3(0f, height, -distance);  // (x=0, y=높이, z=뒤쪽)
        Vector3 desiredPos = focus + yawRot * localOffset;

        // --- 충돌 보정(Raycast) ---
        Vector3 toCam = desiredPos - focus;
        float desiredDist = toCam.magnitude;
        Vector3 dir = toCam.normalized;

        float adjustedDist = desiredDist;
        if (Physics.Raycast(focus, dir, out RaycastHit hit, desiredDist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            adjustedDist = Mathf.Max(hit.distance - collisionBuffer, minCollisionDistance);
        }
        Vector3 finalPos = focus + dir * adjustedDist;

        // --- 스무딩 및 적용 ---
        float tPos = 1f - Mathf.Exp(-positionDamping * Time.unscaledDeltaTime);
        float tRot = 1f - Mathf.Exp(-rotationDamping * Time.unscaledDeltaTime);

        transform.position = Vector3.Lerp(transform.position, finalPos, tPos);

        Quaternion lookRot = Quaternion.LookRotation(focus - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, tRot);
    }
}
