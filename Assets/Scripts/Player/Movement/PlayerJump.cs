using Defines;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerJump : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;   // JumpHeight(=impulse)만 사용
    [SerializeField] private PlayerMove move;
    [SerializeField] private PlayerAnimation playerAnimation;

    [Header("Jump Timing")]
    [Tooltip("지면을 떠난 뒤에도 점프가 입력되면 허용하는 유예 시간(초)")]
    [SerializeField] private float coyoteTime = 0.12f;
    [Tooltip("점프 키를 미리 누른 입력을 버퍼에 저장하는 시간(초)")]
    [SerializeField] private float jumpBufferTime = 0.12f;

    [Header("Variable Jump & Gravity")]
    [SerializeField] private float fallMultiplier = 3.0f;        // 낙하 가속 ↑
    [SerializeField] private float lowJumpMultiplier = 2.6f;     // 짧은 점프 가속 ↑
    [SerializeField] private float riseHoldMultiplier = 1.2f;    // 상승 중(키 유지)에도 살짝 더 무겁게
    [SerializeField] private float airGravityBase = 1.15f;       // 공중 기본 중력 배수
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Misc")]
    [SerializeField] private bool useJumpBuffer = true;
    [SerializeField] private bool useCoyoteTime = true;
    [SerializeField] private bool clampFallSpeed = true;

    [Header("Debug")]
    [SerializeField] AnimationCurve curveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public bool IsJumping { get; private set; }
    private bool isFalling = false;

    // 입력/타이밍
    private bool jumpHeld;                     // 점프 키 현재 유지 여부
    private float lastGroundedTime;            // 마지막으로 지면이었던 시각
    private float lastJumpPressedTime;         // 마지막 점프 입력 시각

    private void Start()
    {
        if (rigid == null)
        {
            this.enabled = false;
            return;
        }

        rigid.useGravity = true; // Unity 중력 사용
    }

    private void OnEnable()
    {
        // 점프 시작(누름)
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsStarted,
            OnJumpPressed
        );

        // 점프 해제(뗌) — 시스템에 따라 ActionPoint 이름이 다를 수 있음
        // 만약 IsCanceled/IsPerformed가 없다면, Update에서 InputManager로 IsPressed를 폴링해도 OK
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsCanceled,
            OnJumpReleased
        );
    }

    private void OnDisable()
    {
        InputManager.Instance?.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsStarted,
            OnJumpPressed
        );

        InputManager.Instance?.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsCanceled,
            OnJumpReleased
        );
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        curveY = EditorGUILayout.CurveField(" ", curveY);
    }
#endif

    private void Update()
    {
        // 지면 타임스탬프 갱신
        if (move.IsGrounded)
        {
            lastGroundedTime = Time.time;
            IsJumping = false;
        }

        // 점프 버퍼 + 코요테 타임 체크
        bool canCoyote = !useCoyoteTime || (Time.time - lastGroundedTime) <= coyoteTime;
        bool buffered = !useJumpBuffer || (Time.time - lastJumpPressedTime) <= jumpBufferTime;

        if (buffered && canCoyote)
        {
            PerformJump(); // 한 번만 소비
            lastJumpPressedTime = -999f;
        }
    }

    private void FixedUpdate()
    {
        Vector3 v = rigid.velocity;

        if (move.IsGrounded && v.y <= 0f)
        {
            if (v.y < 0f)
            {
                v.y = 0f;
                rigid.velocity = v;
            }
            return;
        }

        if (v.y > 0f)
        {
            float mul = jumpHeld ? riseHoldMultiplier : lowJumpMultiplier;
            v += Physics.gravity * (mul - 1f) * Time.fixedDeltaTime;
        }
        else
        {
            float mul = fallMultiplier;    
            v += Physics.gravity * (mul - 1f) * Time.fixedDeltaTime;

            if(isFalling == false)
            {
                isFalling = true;
                playerAnimation.SetFalling(true);
            }
        }

        if (clampFallSpeed && maxFallSpeed > 0f && v.y < -maxFallSpeed)
            v.y = -maxFallSpeed;

        rigid.velocity = v;
    }




    // ===== 입력 핸들러 =====
    private void OnJumpPressed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        jumpHeld = true;
        lastJumpPressedTime = Time.time;

        // 즉시 점프 시도 (버퍼/코요테를 Update에서 처리하므로 여기서는 기록만 해도 되지만
        // 땅 위라면 바로 점프해도 사용감이 좋음)
        if (move.IsGrounded)
        {
            PerformJump();
            lastJumpPressedTime = -999f; // 소비
        }
    }

    private void OnJumpReleased(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        jumpHeld = false;

        if (rigid.velocity.y > 0)
        {
            Vector3 v = rigid.velocity;
            v.y *= 0.5f;
            rigid.velocity = v;
        }
    }

    // ===== 실제 점프 =====
    private void PerformJump()
    {
        // 이미 공중인데 코요테 타임도 끝났으면 무시
        if (!move.IsGrounded && useCoyoteTime && (Time.time - lastGroundedTime) > coyoteTime)
            return;

        // y속도 초기화 후 임펄스(연속 점프/짧은점프 시 안정적)
        Vector3 vel = rigid.velocity;
        if (vel.y < 0f) vel.y = 0f;
        rigid.velocity = vel;

        // JumpHeight는 '임펄스 세기'로 사용
        rigid.AddForce(Vector3.up * status.JumpHeight, ForceMode.Impulse);
        
        IsJumping = true;
        isFalling = false;
        playerAnimation.JumpStart();
    }

    // 기존 공개 API 유지 (외부에서 강제 점프하고 싶을 때 사용)
    public void Jump()
    {
        OnJumpPressed(default);
    }

    public void Jump(float force)
    {
        // 외부 힘으로 가변 점프
        Vector3 vel = rigid.velocity;
        if (vel.y < 0f) vel.y = 0f;
        rigid.velocity = vel;

        rigid.AddForce(Vector3.up * force, ForceMode.Impulse);

        IsJumping = true;
        isFalling = false;
        playerAnimation.JumpStart();
    }

    public void PerformJumpFromGimmick(float force)
    {
        Vector3 vel = rigid.velocity;
        if (vel.y < 0f) vel.y = 0f;
        rigid.velocity = vel;

        rigid.AddForce(Vector3.up * force, ForceMode.Impulse);

        IsJumping = true;
        isFalling = false;
        jumpHeld = true; 

        playerAnimation.JumpStart();
    }


    public void Jump(Vector3 forwardDir, float upForce, float forwardSpeed)
    {
        forwardDir.y = 0f;
        if (forwardDir.sqrMagnitude > 0.0001f)
        {
            forwardDir.Normalize();
        }
        else
        {
            forwardDir = Vector3.zero;
        }

        Vector3 vel = rigid.velocity;
        if (vel.y < 0f) vel.y = 0f;

        if (forwardDir != Vector3.zero)
        {
            vel.x = forwardDir.x * forwardSpeed;
            vel.z = forwardDir.z * forwardSpeed;
        }

        rigid.velocity = vel;

        rigid.AddForce(Vector3.up * upForce, ForceMode.Impulse);

        IsJumping = true;
        isFalling = false;
        playerAnimation.JumpStart();
    }
}
