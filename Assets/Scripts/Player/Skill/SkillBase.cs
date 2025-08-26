using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defines;
using UnityEngine.InputSystem;


public abstract class SkillBase : MonoBehaviour
{
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
