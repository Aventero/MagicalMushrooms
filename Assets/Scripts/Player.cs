using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        StateManager.Instance.PlayerHit += OnPlayerHit;
    }

    private void OnPlayerHit()
    {
        StateManager.Instance.PlayerHealth--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.tag.Equals("Item"))
                return;

        // Add item pickup
        Item item = other.gameObject.GetComponent<Item>();
        Debug.Log("Picked up Item: " + item.Name);

        UIManager.Instance.AddIcon(item.Icon, item.Name);

        Destroy(other.gameObject);
    }
}
