using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<ItemData> items;

    void Start()
    {
        items = new List<ItemData>();
        StateManager.Instance.ItemPickupEvent += this.ItemPickup;
    }

    private void ItemPickup(ItemData newItem)
    {
        items.Add(newItem);
    }

    public ItemData GetItemByName(string itemName)
    {
        return items.Find((item) => item.Name == itemName);
    }

    public bool HasItem(string itemName)
    {
        return GetItemByName(itemName) != null;
    }

    public bool RemoveItem(string itemName)
    {
        ItemData item = GetItemByName(itemName);

        if(item != null)
            return items.Remove(item);

        return false;
    }
}
