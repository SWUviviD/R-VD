using Defines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerMove move;
    [SerializeField] AnimationCurve curveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private PlayerAnimation playerAnimation;

    public bool IsJumping { get; private set; }
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

    private void OnGUI()
    {
        curveY = EditorGUILayout.CurveField(" ", curveY);
    }

    private void FixedUpdate()
    {
        if (move.IsGrounded == false) // 떨어지는 중
        {
            Vector3 realFalling = rigid.velocity;
            realFalling.y -= status.FallingSpeed;
            rigid.velocity = realFalling;
            if (realFalling.y < 0f && isFalling == false)
            {
                isFalling = true;
                playerAnimation.SetFalling(true);
            }
        }
    }

    private void DoJump(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Jump();
    }

    public void Jump()
    {
        if (move.IsGrounded == false) // 떨어지는 중
        {
            return;
        }
        rigid.AddForce(Vector3.up * status.JumpHeight, ForceMode.Impulse);
        playerAnimation.JumpStart();
        playerAnimation.SetFalling(true);
        isFalling = true;
    }

    public void Jump(float force)
    {
        if (move.IsGrounded == false) // 떨어지는 중
        {
            return;
        }
        rigid.AddForce(Vector3.up * force, ForceMode.Impulse);
        playerAnimation.JumpStart();
        playerAnimation.SetFalling(false);
        isFalling = true;
    }

    private void OnDisable()
    {
        InputManager.Instance?.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsStarted,
            DoJump
            );
    }

}
