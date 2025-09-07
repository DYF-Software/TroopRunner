using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "My Assets/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Spawn Settings")]
    [Tooltip("Initial Solider Count")]
    public int initialTroopCount = 10;

    [Header("Movement Settings")]
    public float scrollRange = 5f;
    public float forwardSpeed = 5f;
    public float lateralLerpSpeed = 10f;

    [Header("Path Borders (X Axis)")]
    public float laneHalfWidth = 5f; 
    public float spacingPerTroop = 0.3f; 
}
