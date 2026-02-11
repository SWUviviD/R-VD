using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using UnityEngine.Assertions.Must;
using System.Net.NetworkInformation;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private float maxVelocity;

    [Space]
    [SerializeField] private float rayLength = 1f;
    [SerializeField] private float heightLength = 0.8f;
    [SerializeField] private LayerMask groundLayerMask;

    [Space]
    [SerializeField] private float maxinumAngle = 66f;
    [SerializeField] private float mininumAngle = -46f;
    [SerializeField] private float addMaxRadio = 1.5f;
    [SerializeField] private float addMinRadio = -1f;

    
    public Vector3 MoveDirection { get; private set; }
    public Vector3 CurrentFeetPosition { get; private set; }

    [field: SerializeField] public bool IsGrounded { get; private set; }

    private GameObject currentFloor;
    private IFloorInteractive currentFloorInteractive;

    public UnityEvent OnInteractWithFloorStart { get; private set; } = new UnityEvent();
    public UnityEvent OnInteractWithFloorEnd { get; private set; } = new UnityEvent();

    public UnityEvent<bool> OnMove { get; private set; } = new UnityEvent<bool>();

    public bool IsSlippery { get; set; }

    private void OnEnable()
    {
        currentFloor = null;
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Move),
            InputDefines.ActionPoint.All,
            DoMove
            );
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        RaycastHit hit = default;
        bool isSlope = false;

        bool hitGround = ShootRay(out hit);
        float vy = rigid.velocity.y;

        if(hitGround && vy <= 0.05f)
        {
            if(IsGrounded == false)
            {
                playerAnimation.JumpEnd();
                playerAnimation.SetFalling(false);
            }

            IsGrounded = true;

            Rebound(ref hit);
            FloorInteract(ref hit);
            isSlope = AddSpeed(in hit);
        }
        else
        {
            if (IsGrounded)
                ResetCurrentFloor();
            IsGrounded = false;
        }

        if (status.IsDashing == false)
        {
            Move(isSlope, in hit);
        }
    }

    private void Move(bool isSlope, in RaycastHit hit)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            LogManager.Log("MainCamera NULL");
            return;
        }

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = (cameraRight * MoveDirection.x + cameraForward * MoveDirection.y).normalized;
        Vector3 gravity = Vector3.zero;
        if(isSlope && jump.IsJumping == false)
        {
            move = Vector3.ProjectOnPlane(move, hit.normal).normalized;
        }

        Vector3 v = rigid.velocity;

        if (move.sqrMagnitude > 0)
        {
            // 벽보정
            Vector3 wallOrigin = transform.position + Vector3.up * heightLength * 0.5f;
            float wallRadius = 0.25f;
            float wallDistance = 0.5f;
            if (move.sqrMagnitude > 0f &&
                Physics.SphereCast(wallOrigin, wallRadius, move, out RaycastHit wallHit, 
                wallDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                float angleFromUp = Vector3.Angle(wallHit.normal, Vector3.up); 
                if (angleFromUp > 80f) 
                {
                    move = Vector3.ProjectOnPlane(move, wallHit.normal).normalized;
                }
            }

            Vector3 realMovement = move * (status.MoveSpeed + status.AdditionalMoveSpeed);
            v.x = realMovement.x;
            v.z = realMovement.z;

            Vector3 faceDir = new Vector3(realMovement.x, 0f, realMovement.z);
            if(faceDir.sqrMagnitude > 1e-6f)
            {
                Quaternion targetRot = Quaternion.LookRotation(faceDir, Vector3.up);
                float degPerSec = 720f;
                Quaternion step = Quaternion.RotateTowards(rigid.rotation, targetRot, degPerSec * Time.fixedDeltaTime);
                rigid.MoveRotation(step);
            }
            OnMove?.Invoke(true);
        }
        else
        {

            if(IsGrounded && !jump.IsJumping)
            {
                if (IsSlippery == false)
                {
                    const float groundDecel = 30f;
                    v.x = Mathf.MoveTowards(v.x, 0f, groundDecel * Time.fixedDeltaTime);
                    v.z = Mathf.MoveTowards(v.z, 0f, groundDecel * Time.fixedDeltaTime);

                    if (new Vector2(v.x, v.z).sqrMagnitude < 0.0004f)
                    {
                        v.x = 0f; v.z = 0f;
                    }
                }
                
            }
            OnMove?.Invoke(false);
        }

        v.y = rigid.velocity.y;
        rigid.velocity = v;

        if(IsGrounded)
            CurrentFeetPosition = hit.point;
    }

    private bool ShootRay(out RaycastHit hit)
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        float radius = 0.3f;
        float casDist = rayLength + 0.2f;

        Debug.DrawRay(origin, Vector3.down * casDist, Color.green);
        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out hit,
            casDist,
            groundLayerMask,
            QueryTriggerInteraction.Ignore
            );
    }
    
    private void Rebound(ref RaycastHit _hit)
    {
        Vector3 v = rigid.velocity;
        if(v.y < 0f)
        {
            v.y = 0f;
            rigid.velocity = v;
        }

        return;

        //float currentDist = _hit.distance;
        //float targetDist = heightLength;
        //float deep = currentDist - targetDist;

        //if (Mathf.Abs(deep) < 0.01f) return;

        //Vector3 targetPosition = rigid.position - new Vector3(0f, deep, 0f);

        //rigid.MovePosition(targetPosition);

        //Vector3 v = rigid.velocity;
        //if (v.y < 0)
        //{
        //    v.y = 0;
        //    rigid.velocity = v;
        //}
    }

    private void FloorInteract(ref RaycastHit _hit)
    {
        if(currentFloor != _hit.collider.gameObject)
        {
            currentFloor = _hit.collider.gameObject;

            if(currentFloorInteractive != null)
            {
                currentFloorInteractive.InteractEnd(gameObject);
                OnInteractWithFloorEnd?.Invoke();
            }

            currentFloorInteractive = currentFloor.GetComponentInChildren<IFloorInteractive>();
            if(currentFloorInteractive != null)
            {
                currentFloorInteractive.InteractStart(gameObject);
                OnInteractWithFloorStart?.Invoke();
                return;
            }

            currentFloorInteractive = currentFloor.GetComponentInParent<IFloorInteractive>();
            if (currentFloorInteractive != null)
            {
                currentFloorInteractive.InteractStart(gameObject);
                OnInteractWithFloorStart?.Invoke();
            }
        }
    }

    private void ResetCurrentFloor()
    {
        currentFloor = null;
        CurrentFeetPosition = Vector3.zero;
        if (currentFloorInteractive != null)
        {
            currentFloorInteractive.InteractEnd(gameObject);
            OnInteractWithFloorEnd?.Invoke();
            currentFloorInteractive = null;
        }
    }


    private bool AddSpeed(in RaycastHit hit)
    {
        var myAngle = Vector3.Angle(hit.normal, Vector3.up);

        if (myAngle > maxinumAngle) // 올라가는 중
        {
            status.AdditionalMoveSpeed = status.MoveSpeed * addMinRadio;
            return true;
        }

        if (myAngle <= mininumAngle) // 내려가는 중
        {
            status.AdditionalMoveSpeed = status.MoveSpeed * addMaxRadio;
            return true;
        }

        if(myAngle < 6f && myAngle > -6f)
        {
            status.AdditionalMoveSpeed = 0.0f;
            return false;
        }

        if (myAngle > 6f)
        {
            float ratio = ((int)(myAngle - 6f) / 10 + 1) / 10f;
            status.AdditionalMoveSpeed = status.MoveSpeed * -ratio;
        }
        else if (myAngle < -6f)
        {
            float ratio = ((int)(myAngle + 6f) / 10 - 1) / 10f;
            status.AdditionalMoveSpeed = status.MoveSpeed * -ratio;
        }

        return true;
    }

    private void OnDisable()
    {
        InputManager.Instance?.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Move),
            InputDefines.ActionPoint.All,
            DoMove
            );
    }

    private void DoMove(InputAction.CallbackContext obj)
    {
        MoveDirection = obj.ReadValue<Vector2>();
    }

    public void  SetPosition(Vector3 _newPos)
    {
        rigid.position = _newPos + Vector3.up * (rayLength - heightLength + 0.05f);
    }

    public void SetPositionByTransform(Vector3 _newPos)
    {
        transform.position = _newPos + Vector3.up * (rayLength - heightLength + 0.05f);
    }

    public Vector3 GetPosition()
    {
        return transform.position - transform.up * heightLength;
    }

    public void SetRotation(Vector3 _newRot)
    {
        transform.rotation = Quaternion.Euler(_newRot);
    }

    public Vector3 GetRotation()
    {
        return transform.rotation.eulerAngles;
    }

    public void StopMoving()
    {
        rigid.velocity = Vector3.up * rigid.velocity.y;
        OnMove?.Invoke(false);
    }

    public void MoveTo(Transform target)
    {
        rigid.velocity = Vector3.up * rigid.velocity.y;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
