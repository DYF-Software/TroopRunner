using UnityEngine;

public class DisableLogs : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += SuppressLogs;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= SuppressLogs;
    }

    void SuppressLogs(string condition, string stackTrace, LogType type)
    {

    }
}
