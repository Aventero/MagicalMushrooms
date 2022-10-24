using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Witch"))
        {
            StateManager.Instance.PlayerHealth--;
            
            // trigger player hit event
            StateManager.Instance.PlayerHit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.tag.Equals("Item"))
                return;

        // Add item pickup
        Debug.Log("Picked up Item");
        Destroy(other.gameObject);
    }
}
