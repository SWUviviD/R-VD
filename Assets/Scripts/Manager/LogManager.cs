using UnityEngine;

public static class LogManager
{
    public static void Log(string msg)
    {
#if UNITY_EDITOR || DEVELOPMENT_MODE
        Debug.Log(msg);
#endif
    }

    public static void LogWarning(string msg)
    {
#if UNITY_EDITOR || DEVELOPMENT_MODE
        Debug.LogWarning(msg);
#endif
    }

    public static void LogError(string msg)
    {
#if UNITY_EDITOR || DEVELOPMENT_MODE
        Debug.LogError(msg);
#endif
    }
}
