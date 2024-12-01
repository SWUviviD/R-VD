using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;

    private Vector3 dashDirection;
    private WaitForSeconds waitForDashSeconds;

    [SerializeField]
    private float dashCoolTime = 1.5f;
    private float elaspedTime = 0.0f;
    private bool canDash = true;

    public UnityEvent<bool> OnDashEvent = new UnityEvent<bool>();

    private void Start()
    {
        elaspedTime = 0.0f;
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

        if (canDash == false)
        {
            elaspedTime += Time.deltaTime;
            if (elaspedTime >= dashCoolTime)
            {
                elaspedTime -= dashCoolTime;
                canDash = true;
            }
        }
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
        if (canDash == false)
            return;

        status.IsDashing = true;
        OnDashEvent?.Invoke(true);
        canDash = false;

        dashDirection = rigid.transform.forward * status.DashSpeed;
        dashDirection.y = 0f;

        StartCoroutine(EndDashing());
    }

    private IEnumerator EndDashing()
    {
        yield return waitForDashSeconds;
        status.IsDashing = false;
        OnDashEvent?.Invoke(false);
        rigid.velocity = Vector3.zero;
    }
}
