using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerHealth = 3;

    void Start()
    {
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.tag.Equals("Witch"))
            playerHealth--;
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
