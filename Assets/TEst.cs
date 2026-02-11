using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEst : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("awake");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;

        Debug.Log("enable");
    }

    private void Start()
    {
        Debug.Log("start");
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SceneLoaded");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

    }

    private void OnDisable()
    {
        Debug.Log("destroy");
    }

    private void OnDestroy()
    {
        Debug.Log("destroy");
    }
}
