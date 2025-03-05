using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVaseControll : MonoBehaviour
{
    public bool waterLevelOne;
    public bool waterLevelTwo;

    private void Start()
    {
        waterLevelOne = false; waterLevelTwo = false;
        LogManager.Log("" + waterLevelOne + waterLevelTwo);
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

        LogManager.Log("" + waterLevelOne + waterLevelTwo);
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

        LogManager.Log("" + waterLevelOne + waterLevelTwo);
    }
}
