using UnityEngine;
using UnityEngine.InputSystem;
using Defines;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] PlayerStatus status;
    
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
        bool hasControl = (moveDirection != Vector3.zero);
        //Debug.Log("moveDirection : " + moveDirection);
        if (hasControl)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            rigidbody.MovePosition(rigidbody.position + moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime);
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
        Vector2 input = obj.ReadValue<Vector2>();
        if (input != null)
        {
            moveDirection = new Vector3(input.x, 0f, input.y);
        }
    }
}
