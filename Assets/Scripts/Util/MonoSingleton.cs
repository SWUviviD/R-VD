using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool isInit = false;
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (isInit)
        {
            if (_instance == this) return;
            
            Destroy(gameObject);
            return;
        }
        
        isInit = true;
        Init();
    }

    private void OnDestroy()
    {
        isInit = false;
        _instance = null;
    }

    protected virtual void Init() { }
}
