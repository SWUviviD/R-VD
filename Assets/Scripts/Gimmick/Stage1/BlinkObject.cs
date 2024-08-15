using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    [SerializeField] private GameObject blinkObject;
    [SerializeField] private float disableTime;
    [SerializeField] private float enableTime;
    private float elapsedTime;

    private void Start()
    {
        elapsedTime = 0f;
    }

    private void FixedUpdate()
    {
        if(blinkObject.activeSelf)
        {
            if(CheckElapsedTime(Time.deltaTime, enableTime))
            {
                blinkObject.SetActive(false);
                elapsedTime = 0f;
            }
        }
        else
        {
            if (CheckElapsedTime(Time.deltaTime, disableTime))
            {
                blinkObject.SetActive(true);
                elapsedTime = 0f;
            }
        }
    }

    private bool CheckElapsedTime(float deltaTime, float checkTime)
    {
        elapsedTime += deltaTime;
        if (elapsedTime > checkTime)
        {
            return true;
        }
        return false;
    }
}
