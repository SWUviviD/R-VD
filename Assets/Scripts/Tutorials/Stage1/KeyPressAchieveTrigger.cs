using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Defines.InputDefines;

public class KeyPressAchieveTrigger : TutorialTargetAchieveTrigger
{
    [SerializeField] private string KeyName = SkillType.StarHunt.ToString();

    public override void ActivateTrigger()
    {
        InputManager.Instance.AddInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, KeyName),
            ActionPoint.IsStarted, (InputAction.CallbackContext _ctx) => Achieved());
    }

    protected override void DisableTrigger()
    {
        InputManager.Instance.RemoveInputEventFunction(
            new InputActionName(ActionMapType.PlayerActions, SkillType.StarHunt.ToString()),
            ActionPoint.IsStarted, (InputAction.CallbackContext _ctx) => Achieved());
    }
}
