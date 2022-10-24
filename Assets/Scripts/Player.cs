using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Witch"))
        {
            StateManager.Instance.PlayerHealth--;
            
            // Trigger player hit event
            StateManager.Instance.PlayerHit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.tag.Equals("Item"))
                return;

        // Add item pickup
        Item item = other.gameObject.GetComponent<Item>();
        Debug.Log("Picked up Item: " + item.Name);

        Destroy(other.gameObject);
    }
}
