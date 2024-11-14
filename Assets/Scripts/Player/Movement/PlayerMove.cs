using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using UnityEngine.Assertions.Must;
using System.Net.NetworkInformation;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerJump jump;
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

    public bool IsGrounded { get; private set; }

    private GameObject currentFloor;

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
        RaycastHit hit;
        bool isSlope = false;
        IsGrounded = ShootRay(out hit);
        if(IsGrounded)
        {
            Rebound(ref hit);
            FloorInteract(ref hit);
            isSlope = AddSpeed(in hit);
        }
        else
        {
            ResetCurrentFloor();
        }

        if (status.IsDashing == false)
        {
            Move(isSlope, in hit);
        }
    }

    private void Move(bool isSlope, in RaycastHit hit)
    {
        Vector3 move = new Vector3(MoveDirection.x, 0f, MoveDirection.y);
        Vector3 gravity = Vector3.zero;
        if(isSlope && jump.IsJumping == false)
        {
            move = Vector3.ProjectOnPlane(move, hit.normal);
            gravity = Vector3.down * Mathf.Abs(rigid.velocity.y);
        }

        if (move.sqrMagnitude > 0)
        {
            Vector3 realMovement = move.normalized * (status.MoveSpeed + status.AdditionalMoveSpeed) + gravity ;
            realMovement.y = rigid.velocity.y;
            rigid.velocity = realMovement;
            realMovement.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(realMovement);
        }
        rigid.velocity *= 0.9f;
    }

    private bool ShootRay(out RaycastHit hit)
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        return Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayerMask);
    }
    
    private void Rebound(ref RaycastHit _hit)
    {
        float deep = _hit.distance - heightLength;
        rigid.MovePosition(rigid.transform.position - new Vector3(0f, deep, 0f));
    }

    private void FloorInteract(ref RaycastHit _hit)
    {
        if(currentFloor != _hit.collider.gameObject)
        {
            currentFloor = _hit.collider.gameObject;
            
            IFloorInteractive interactiveObj = null;
            
            interactiveObj = currentFloor.GetComponentInChildren<IFloorInteractive>();
            if(interactiveObj != null)
            {
                interactiveObj.Interact(gameObject);
                return;
            }

            interactiveObj = currentFloor.GetComponentInParent<IFloorInteractive>();
            if (interactiveObj != null)
            {
                interactiveObj.Interact(gameObject);
            }
        }
    }

    private void ResetCurrentFloor()
    {
        currentFloor = null;
    }


    private bool AddSpeed(in RaycastHit hit)
    {
        var myAngle = Vector3.Angle(transform.forward, hit.normal) - 90;

        LogManager.Log(myAngle.ToString());
        if(myAngle > maxinumAngle) // 올라가는 중
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
}
