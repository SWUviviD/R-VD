using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneShortcut
{
    [MenuItem("Vivid/OpenEditorScene #&1")]
    private static void OpenEditorScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LevelEditor.unity", OpenSceneMode.Single);
    }
    
    [MenuItem("Vivid/OpenMainScene #&2")]
    private static void OpenMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
    }
}
