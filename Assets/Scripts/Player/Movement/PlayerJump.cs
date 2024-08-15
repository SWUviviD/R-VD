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
    [SerializeField] AnimationCurve curveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

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

    private void OnGUI()
    {
        curveY = EditorGUILayout.CurveField(" ", curveY);
    }

    private void FixedUpdate()
    {
        // 점프중 아님
        if (isJumping == false && isFalling == false)
        {
            elapsedTime = 0f;
            return;
        }

        if(isFalling == true)
        {
            currentPos -= Time.deltaTime * status.FallingSpeed;

            jumpPos.Set(rigid.position.x, currentPos, rigid.position.z);
            rigid.position = jumpPos;
            return;
        }

        if(isJumping == true)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= status.JumpSpeed)
            {
                isJumping = false;
                isFalling = true;
                elapsedTime = 0f;
                Debug.Log("JumpEnd");
                return;
            }

            currentPos = Mathf.Lerp(startPos, endPos, curveY.Evaluate(elapsedTime / status.JumpSpeed));
            jumpPos.Set(rigid.position.x, currentPos, rigid.position.z);
            rigid.position = jumpPos;
            return;
        }
    }

    private void DoJump(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Jump();
    }

    public void Jump()
    {
        if (isJumping == true || isFalling == true)
            return;

        isJumping = true;
        isFalling = false;

        currentPos = transform.position.y;
        startPos = transform.position.y;
        endPos = currentPos + status.JumpHeight;
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
        InputManager.Instance?.RemoveInputEventFunction(
            new Defines.InputDefines.InputActionName(Defines.InputDefines.ActionMapType.PlayerActions, InputDefines.Jump),
            InputDefines.ActionPoint.IsStarted,
            DoJump
            );
    }

}
