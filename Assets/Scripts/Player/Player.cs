using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
