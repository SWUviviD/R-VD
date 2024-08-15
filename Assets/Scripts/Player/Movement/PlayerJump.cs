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

    private bool isJumping = false;
    private bool isFalling = false;

    private float startPos;
    private float endPos;
    private float currentPos;
    private Vector3 jumpPos;
    private float elapsedTime = 0f;

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

    private void FixedUpdate()
    {
        // 점프중 아님
        if (isJumping == false && isFalling == false)
            return;

        if(isFalling == true)
        {
            elapsedTime += Time.deltaTime;

            currentPos -= elapsedTime / jumpSpeed;

            jumpPos.Set(0f, currentPos, 0f);
            rigid.MovePosition(transform.position + jumpPos);
            return;
        }

        if(isJumping == true)
        {
            elapsedTime += Time.deltaTime;

            if (Mathf.Abs(currentPos - endPos) <= 0.5f)
            {
                isJumping = false;
                isFalling = true;
                elapsedTime = 0f;
                return;
            }

            currentPos = Mathf.Lerp(currentPos, endPos, elapsedTime / jumpSpeed);
            jumpPos.Set(0f, currentPos, 0f);
            rigid.MovePosition(transform.position + jumpPos);
            return;
        }
    }

    private void DoJump(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Jump();
    }

    public void Jump()
    {
        if (isJumping == true)
            return;

        isJumping = true;
        isFalling = false;

        currentPos = transform.position.y;
        endPos = currentPos + jumpHeight;
        Debug.Log(currentPos + " " + endPos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isJumping = false;
            isFalling = false;
        }
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
