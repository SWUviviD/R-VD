using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;

    private Vector3 dashDirection;
    private WaitForSeconds waitForDashSeconds;

    private void Start()
    {
        waitForDashSeconds = new WaitForSeconds(status.DashTime);
    }

    private void OnEnable()
    {
        InputManager.Instance.AddInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Dash),
            Defines.InputDefines.ActionPoint.IsStarted,
            OnDash);
    }

    private void FixedUpdate()
    {
        if ((status.IsDashing) == true)
        {
            rigid.velocity = dashDirection;
        }
        rigid.velocity *= 0.5f;
    }

    private void OnDisable()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, Defines.InputDefines.Dash),
            Defines.InputDefines.ActionPoint.IsStarted,
            OnDash);
    }

    private void OnDash(InputAction.CallbackContext _context)
    {
        status.IsDashing = true;

        dashDirection = rigid.transform.forward * status.DashSpeed;
        dashDirection.y = 0f;

        StartCoroutine(EndDashing());
    }

    private IEnumerator EndDashing()
    {
        yield return waitForDashSeconds;
        status.IsDashing = false;
        rigid.velocity = Vector3.zero;
    }
}
