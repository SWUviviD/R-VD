using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    private void Start()
    {
        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Title); /*, false, 
            (Scene, LoadSceneMode) => GameManager.Instance.ConnectCanvas());*/
    }
}
