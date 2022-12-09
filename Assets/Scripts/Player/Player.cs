using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        //StateManager.Instance.PlayerHitEvent += OnPlayerHit;
    }

    private void OnPlayerHit()
    {
        //StateManager.Instance.PlayerHealth--;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Witch")
        {
            StateManager.Instance.WitchConeOnPlayer = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Witch View Cone
        if (other.tag == "Witch")
        {
            Debug.Log("TRUE");
            StateManager.Instance.WitchConeOnPlayer = true;
        }

        if(!other.gameObject.GetComponent<Interactable>())
            return;

        // Add item pickup
        Item item = other.gameObject.GetComponent<Item>();

        StateManager.Instance.ItemPickupEvent(item);

        Destroy(other.gameObject);
    }
}
