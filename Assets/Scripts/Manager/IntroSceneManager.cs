using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : MonoBehaviour
{
    private void Start()
    {
        SceneLoadManager.Instance.LoadScene(SceneDefines.Scene.Title);
    }
}
