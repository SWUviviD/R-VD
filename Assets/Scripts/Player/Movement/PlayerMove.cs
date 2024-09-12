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

    [SerializeField] private float moveDrag = 0.9f;

    
    private Vector3 moveDirection;

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
        Vector3 move = new Vector3(moveDirection.x, 0f, moveDirection.y);
        if (move.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(move);
            rigid.velocity = move.normalized * status.MoveSpeed;
        }
        rigid.velocity *= 0.9f;

        RaycastHit hit;
        bool result = ShootRay(out hit);
        if(result)
        {
            Rebound(ref hit);
        }

    }

    private bool ShootRay(out RaycastHit hit)
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        return Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayerMask);
    }
    
    private void Rebound(ref RaycastHit _hit)
    {
        float deep = _hit.distance - heightLength;
        if (deep < 0)
        {
            rigid.MovePosition(transform.position - new Vector3(0f, deep, 0f));
        }
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
        moveDirection = obj.ReadValue<Vector2>();
    }
}
