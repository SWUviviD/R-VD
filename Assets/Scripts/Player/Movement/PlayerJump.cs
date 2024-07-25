using Defines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{

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
