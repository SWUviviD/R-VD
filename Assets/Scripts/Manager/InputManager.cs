using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Actions actions;

    private void Awake()
    {
        if (actions == null)
            actions = new Actions();
    }

    public static void ChangeInputEventOption(string actionName, int bindingIndex)
    {
        Debug.Log("ChangeInputEventOption : " + actionName);
        InputAction inputAction = actions.asset.FindAction(actionName);
        if (inputAction == null || inputAction.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if (inputAction.bindings[bindingIndex].isComposite)
        {
            Debug.Log("isComposite");
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < inputAction.bindings.Count && inputAction.bindings[firstPartIndex].isPartOfComposite)
            {
                Debug.Log("DoChange for Composite");
                DoChange(inputAction, firstPartIndex, true);
            }
        }
        else
        {
            Debug.Log("DoChange");
            DoChange(inputAction, bindingIndex, false);
        }
            
    }

    public static void DoChange(InputAction actionToChange, int bindingIndex, bool allCompositeParts)
    {
        if (actionToChange == null)
            return;

        string statusText;
        statusText = $"Press a {actionToChange.expectedControlType} for {actionToChange.bindings[bindingIndex].name}";
        Debug.Log(statusText);

        actionToChange.Disable();

        var change = actionToChange.PerformInteractiveRebinding(bindingIndex);

        change.OnComplete(operation =>
        {
            actionToChange.Enable();
            operation.Dispose();

            if(allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToChange.bindings.Count && actionToChange.bindings[nextBindingIndex].isPartOfComposite)
                    DoChange(actionToChange, nextBindingIndex, allCompositeParts);
            }

            SaveBindingOverride(actionToChange);
        });

        change.OnCancel(operation =>
        {
            actionToChange.Enable();
            operation.Dispose();
        });

        change.Start();
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for(int i=0; i<action.bindings.Count;i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        if (actions == null)
            actions = new Actions();

        InputAction inputAction = actions.asset.FindAction(actionName);

        for(int i =0; i< inputAction.bindings.Count;i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(inputAction.actionMap + inputAction.name + i)))
                inputAction.ApplyBindingOverride(i, PlayerPrefs.GetString(inputAction.actionMap + inputAction.name + i));
        }
    }

    public delegate void delegateFunc(InputAction.CallbackContext obj);

    public static void AddInputEventFunction(string actionName, IPlayerActions instance, bool isCanceled = false, bool isPerformed = false, bool isStarted = true)
    {
        InputAction inputAction = actions.asset.FindAction(actionName);
        if (isStarted)
            inputAction.started += instance.DoMove;
        if (isPerformed)
            //inputAction.performed +=func;
        if(isCanceled)
            //inputAction.canceled += func;
        inputAction.Enable();
    }

    public void RemoveInputEventFunction(string actionName, delegateFunc func, bool isCanceled = false, bool isPerformed = false, bool isStarted = true)
    {
        //특정 이벤트에 붙어있는 특정 함수만 등록 해제
        InputAction inputAction = actions.asset.FindAction(actionName);
        if (isStarted)
            //inputAction.started -= func;
        if (isPerformed)
            //inputAction.performed -=func;
        if (isCanceled)
            //inputAction.canceled -= func;
        inputAction.Disable();
    }

    private void RemoveAllEventFunction(string actionName)
    {
        //모든 액션 네임 아니고 특정 액션 네임 받아오면
        //거기에 붙어있는 함수 다 제거
        InputAction inputAction = actions.asset.FindAction(actionName);
        inputAction.Reset();
        //RemoveInputEventFunction(actionName, );
    }
}
