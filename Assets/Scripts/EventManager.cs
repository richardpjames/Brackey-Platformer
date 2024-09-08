using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    // These are the game events
    public static Action<int> OnScoreUpdated;
    public static Action OnCoinCollected;
    public static Action OnPlayerDeath;
    public static Action OnRespawnTriggered;
    private void Awake()
    {
        // Ensure that this is the only instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
