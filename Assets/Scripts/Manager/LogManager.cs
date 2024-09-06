using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    public void Log(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(msg);
#endif
    }

    public void LogWarning(string msg)
    {
#if UNITY_EDITOR
        Debug.LogWarning(msg);
#endif
    }

    public void LogError(string msg)
    {
#if UNITY_EDITOR
        Debug.LogError(msg);
#endif
    }
}
