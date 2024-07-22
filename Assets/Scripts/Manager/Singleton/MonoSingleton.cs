using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<T>();
            }

            if (_instance == null)
            {
                GameObject go = new GameObject();
                _instance = go.AddComponent<T>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            Init();
        }
    }

    protected virtual void Init() { }
}
