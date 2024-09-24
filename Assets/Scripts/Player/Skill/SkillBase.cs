using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;
using UnityEngine.InputSystem;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] private InputDefines.SkillType skillType;


    private void OnEnable()
    {
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, skillType.ToString()),
            InputDefines.ActionPoint.IsStarted, OnSkillStart);
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, skillType.ToString()),
            InputDefines.ActionPoint.IsPerformed, OnSkill);
        InputManager.Instance.AddInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, skillType.ToString()),
            InputDefines.ActionPoint.IsCanceled, OnSkillStop);
    }

    private void OnDisable()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, skillType.ToString()),
            InputDefines.ActionPoint.IsStarted, OnSkillStart);
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, skillType.ToString()),
            InputDefines.ActionPoint.IsPerformed, OnSkillStart);
        InputManager.Instance.RemoveInputEventFunction(
            new InputDefines.InputActionName(InputDefines.ActionMapType.PlayerActions, skillType.ToString()),
            InputDefines.ActionPoint.IsCanceled, OnSkillStart);
    }

    /// <summary>
    /// 스킬 키가 눌렸을 때
    /// </summary>
    /// <param name="_playerStatus"></param>
    /// <returns></returns>
    public abstract void OnSkillStart(InputAction.CallbackContext _playerStatus);
    /// <summary>
    /// 스킬 키가 눌리고 있을 때
    /// </summary>
    /// <param name="_playerStatus"></param>
    /// <returns></returns>
    public abstract void OnSkill(InputAction.CallbackContext _playerStatus);
    /// <summary>
    /// 스킬 키가 떼였을 때
    /// </summary>
    /// <param name="_playerStatus"></param>
    /// <returns></returns>
    public abstract void OnSkillStop(InputAction.CallbackContext _playerStatus);
}
