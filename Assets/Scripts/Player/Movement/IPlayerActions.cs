using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerActions
{
    void DoMove(InputAction.CallbackContext obj);
}
