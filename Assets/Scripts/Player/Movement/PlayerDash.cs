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
    private WaitForFixedUpdate waitForFixedUpate = new WaitForFixedUpdate();

    [SerializeField]
    private float dashCoolTime = 1.5f;
    private float elaspedTime = 0.0f;
    private bool canDash = true;
    private float cachedY;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onDashGauge = new UnityEvent<float>();
    public UnityEvent<bool> OnDashEvent = new UnityEvent<bool>();

    private void Start()
    {
        elaspedTime = 0.0f;
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
            Vector3 v = dashDirection * status.DashSpeed;
            v.y = cachedY;
            rigid.velocity = v;
        }

        if (canDash == false)
        {
            elaspedTime += Time.deltaTime;
            onDashGauge?.Invoke(Mathf.Clamp01(elaspedTime / dashCoolTime));
            GameManager.Instance.HpUI?.SetDashGauge(Mathf.Clamp01(elaspedTime / dashCoolTime));
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

        canDash = false;

        dashDirection = rigid.transform.forward; 
        dashDirection.y = 0; 
        dashDirection.Normalize();

        cachedY = rigid.velocity.y;

        StartCoroutine(EndDashing());
    }

    private IEnumerator EndDashing()
    {
        status.IsDashing = true;
        OnDashEvent?.Invoke(true);

        bool prevGravity = rigid.useGravity;
        rigid.useGravity = false;

        float t = 0f;
        while(t < status.DashTime)
        {
            t += Time.fixedDeltaTime;
            yield return waitForFixedUpate;
        }

        rigid.useGravity = prevGravity;
        status.IsDashing = false;
        OnDashEvent?.Invoke(false);

        var v = rigid.velocity;
        v.y = cachedY;
        v.z = 0f;
        rigid.velocity = v;
    }
}
