using UnityEngine;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputActionReference;

    [SerializeField]
    private int selectedBinding;
    [SerializeField]
    private InputBinding.DisplayStringOptions displayStringOptions;
    [SerializeField]
    private InputBinding inputBinding;
    private int bindingIndex;

    private string actionName;

    private string rebindText;

    private void OnEnable()
    {
        if(inputActionReference!=null)
        {
            InputManager.LoadBindingOverride(actionName);
            GetBindingInfo();
            UpdateUI();
        }
    }

    private void OnValidate()
    {
        if (inputActionReference == null)
            return;

        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if (inputActionReference.action != null)
            actionName = inputActionReference.action.name;

        if(inputActionReference.action.bindings.Count>selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    private void UpdateUI()
    {
        if(rebindText != null)
        {
            if (Application.isPlaying)
            {
                //grab info from Input manager
            }
            else
            {
                rebindText = inputActionReference.action.GetBindingDisplayString(bindingIndex);
                Debug.Log(rebindText);
            }
        }
    }

    public void DoChange()
    {
        InputManager.ChangeInputEventOption(actionName, bindingIndex);
    }
}
