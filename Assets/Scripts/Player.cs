using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        StateManager.Instance.PlayerHitEvent += OnPlayerHit;
    }

    private void OnPlayerHit()
    {
        StateManager.Instance.PlayerHealth--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.GetComponent<Interactable>())
            return;

        // Add item pickup
        Item item = other.gameObject.GetComponent<Item>();

        StateManager.Instance.ItemPickupEvent(item);

        Destroy(other.gameObject);
    }
}
