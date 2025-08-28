using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;

public class WaterVaseControll : SkillBase
{
    public bool remainingUsage;


    private void Start()
    {
        remainingUsage = false;
    }

    public void watermove()
    {
        if (remainingUsage)
        {
            remainingUsage = false;
        }
        else
        {
            remainingUsage = true;
        }
    }

    public override void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {
        //watermove();
    }

    public override void OnSkill(InputAction.CallbackContext _playerStatus)
    {
        
    }

    public override void OnSkillStop(InputAction.CallbackContext _playerStatus)
    {
        
    }
}
