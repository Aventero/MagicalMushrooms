using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public int PlayerHealth = 3;

    // Camera
    public bool isLockedOnWitchHead = false;

    // Custom Events
    public delegate void PlayerHitCallback();
    public PlayerHitCallback PlayerHit;
}
