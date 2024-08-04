using UnityEngine;
using UnityEngine.InputSystem;
using Defines;
using System;
using static Defines.InputDefines;

public class InputManager : MonoSingleton<InputManager>
{
    [SerializeField] PlayerInput playerInput;

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);
    }

    public bool AddInputEventFunction(InputDefines.InputActionName actionName, InputDefines.ActionPoint actionPoint, Action<InputAction.CallbackContext> instance)
    {
        InputAction inputAction = GetInputActionMapByType(actionName);
        if (inputAction == null)
            return false;

        switch (actionPoint)
        {
            case InputDefines.ActionPoint.IsStarted:
                inputAction.started += instance;
                break;
            case InputDefines.ActionPoint.IsPerformed:
                inputAction.performed += instance;
                break;
            case InputDefines.ActionPoint.IsCanceled:
                inputAction.canceled += instance;
                break;
            case InputDefines.ActionPoint.All:
                inputAction.started += instance;
                inputAction.performed += instance;
                inputAction.canceled += instance;
                break;
        }

        return true;
    }

    public bool RemoveInputEventFunction(InputDefines.InputActionName actionName, InputDefines.ActionPoint actionPoint, Action<InputAction.CallbackContext> instance)
    {
        //특정 이벤트에 붙어있는 특정 함수만 등록 해제
        InputAction inputAction = GetInputActionMapByType(actionName);
        if (inputAction == null)
            return false;

        switch (actionPoint)
        {
            case InputDefines.ActionPoint.IsStarted:
                inputAction.started -= instance;    
                break;
            case InputDefines.ActionPoint.IsPerformed:
                inputAction.performed -= instance;
                break;
            case InputDefines.ActionPoint.IsCanceled:
                inputAction.canceled -= instance;
                break;
            case InputDefines.ActionPoint.All:
                inputAction.started -= instance;
                inputAction.performed -= instance;
                inputAction.canceled -= instance;
                break;
        }

        return true;
    }

    public void RemoveAllEventFunction(InputDefines.InputActionName inputPoint)
    {
        //모든 액션 네임 아니고 특정 액션 네임 받아오면
        //거기에 붙어있는 함수 다 제거
        InputAction inputAction = GetInputActionMapByType(inputPoint);
        if (inputAction == null)
            return;

        inputAction.Reset();
    }

    public void EnableAction(InputDefines.InputActionName actionPoint, bool isEnable)
    {
        InputAction inputAction = GetInputActionMapByType(actionPoint);
        if (isEnable)
        {
            inputAction.Enable();
        }
        else
        {
            inputAction.Disable();
        }
    }

    public void EnableActionMap(InputDefines.ActionMapType mapName, bool isEnable)
    {
        InputActionMap map = playerInput.actions.FindActionMap(mapName.ToString());
        if (isEnable)
        {
            map.Enable();
        }
        else
        {
            map.Disable();
        }
    }

    public InputAction GetInputActionMapByType(InputDefines.InputActionName actionPoint)
    {
        return playerInput?.actions.FindActionMap(actionPoint.MapType.ToString())?.FindAction(actionPoint.ActionName);
    }

    // * 키 변경을 위한 함수 추가될 가능성 있음
}
