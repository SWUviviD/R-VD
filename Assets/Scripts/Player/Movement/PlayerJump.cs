using Defines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private Rigidbody rigid;

    private bool isJumping;
    private Vector3 jumpPos;

    private void Start()
    {
        if (rigid == null)
            this.enabled = false;
    }

    private void OnEnable()
    {
        InputManager.Instance.AddInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsStarted,
            DoJump
            );

    }

    private void DoJump(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isJumping == true)
            return;

        isJumping = true;
        StartCoroutine(Jump());
    }

    private IEnumerator Jump()
    {
        float elapedTime = 0.0f;

        float startYPos = transform.position.y;
        float endYPos = startYPos + jumpHeight;
        float currentYPos = startYPos;
        bool isGoingUp = true;

        while(isJumping)
        {
            elapedTime += Time.deltaTime;

            if (Mathf.Abs(endYPos - currentYPos) <= 0.5f)
            {
                break;
            }

            currentYPos = Mathf.Lerp(currentYPos, endYPos, elapedTime / jumpSpeed);
            jumpPos.Set(transform.position.x, currentYPos, transform.position.z);
            transform.position = jumpPos;

            yield return null;
        }

        elapedTime = 0.0f;

        while(isJumping)
        {
            elapedTime += Time.deltaTime;

            currentYPos -= elapedTime / jumpSpeed;

            jumpPos.Set(transform.position.x, currentYPos, transform.position.z);
            transform.position = jumpPos;

            yield return null;
        }

        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
            isJumping = false;
    }

    private void OnDisable()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsStarted,
            DoJump
            );
    }

}
