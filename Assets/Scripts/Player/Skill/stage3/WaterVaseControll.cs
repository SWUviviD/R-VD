using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVaseControll : MonoBehaviour
{
    public bool waterLevelOne;
    public bool waterLevelTwo;

    public GameObject waterMoveEffect;

    private void Start()
    {
        waterLevelOne = false; waterLevelTwo = false;
    }

    public void addWater()
    {
        GameObject watermove = Instantiate(waterMoveEffect, transform.position, Quaternion.identity);
        watermove.transform.SetParent(transform);
        Destroy(watermove, 2f);

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
}
