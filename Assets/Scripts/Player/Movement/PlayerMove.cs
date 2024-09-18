using UnityEngine;
using UnityEngine.InputSystem;
using Defines;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private float maxVelocity;

    [SerializeField] private float rayLength = 1f;
    [SerializeField] private float heightLength = 0.8f;
    [SerializeField] private LayerMask groundLayerMask;
    
    public Vector3 MoveDirection { get; private set; }

    public bool IsGrounded { get; private set; }

    private void OnEnable()
    {
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, InputDefines.Move),
            InputDefines.ActionPoint.All,
            DoMove
            );
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (status.IsDashing == false)
        {
            Move();
        }

        RaycastHit hit;
        IsGrounded = ShootRay(out hit);
        if(IsGrounded)
        {
            Rebound(ref hit);
        }

    }

    private void Move()
    {
        Vector3 move = new Vector3(MoveDirection.x, 0f, MoveDirection.y);
        if (move.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(move);
            Vector3 realMovement = move.normalized * status.MoveSpeed;
            realMovement.y = rigid.velocity.y;
            rigid.velocity = realMovement;
            LogManager.Log(rigid.velocity.ToString());
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
