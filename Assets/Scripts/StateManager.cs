using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public const int PlayerMaxHealth = 3;
    public int PlayerHealth = PlayerMaxHealth;
    public bool OnElevator = false;

    // Witch
    public bool WitchConeOnPlayer = false;

    // Camera
    public bool isLockedOnWitchHead = false;

    public bool InMenu = false;

    // Custom Events
    public delegate void PlayerHitCallback();
    public UnityAction PlayerHitEvent;
    public UnityAction AllItemsCollectedEvent;

    public delegate void ItemPickupCallBack(Item item);
    public ItemPickupCallBack ItemPickupEvent;
}
