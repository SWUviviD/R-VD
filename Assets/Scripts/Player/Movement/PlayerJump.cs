using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Actions actions;

    private Rigidbody rb;

    private void OnEnable()
    {
        actions = InputManager.actions;

        actions.PlayerActions.Jump.started += DoJump;
        actions.PlayerActions.Enable();

        rb = this.GetComponent<Rigidbody>();
    }

    private void DoJump(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}
