using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStringManagerSys : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (StringManagerSystem.Instance != null)
        {
            string title = StringManagerSystem.Instance["UI.Title"];
            LogManager.Log(title);
        }
        else
        {
            LogManager.LogError("StringManager instance is not initialized.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
