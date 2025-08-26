using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Defines;

public class WaterVaseControll : SkillBase
{
    public bool waterLevelOne;
    public bool waterLevelTwo;


    private void Start()
    {
        waterLevelOne = false;
        waterLevelTwo = false;
    }

    public void addWater()
    {
        if (waterLevelOne)
        {
            waterLevelTwo = true;
        }
        else
        {
            waterLevelOne = true;
        }
    }

    public void removeWater()
    {
        if (waterLevelTwo)
        {
            waterLevelTwo = false;
        }
        else if (waterLevelOne)
        {
            waterLevelOne = false;
        }
    }

    public override void OnSkillStart(InputAction.CallbackContext _playerStatus)
    {
        addWater();
    }

    public override void OnSkill(InputAction.CallbackContext _playerStatus)
    {
        
    }

    public override void OnSkillStop(InputAction.CallbackContext _playerStatus)
    {
        removeWater();
    }
}
