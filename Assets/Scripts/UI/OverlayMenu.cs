using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverlayMenu : MonoBehaviour
{
    [SerializeField] private Sprite HealthSprite;
    [SerializeField] private TMP_Text itemCounterText;
    public GameObject InteractionText;
    public GameObject IconParent;

    private GameObject[] healthObjects;

    private List<Sprite> pickedUpItemsSprites;
    private GameObject[] pickedUpItems; // List for displaying the item sprites

    private int amountOfItems;
    private int pickedUpItemsCounter = 0;

    public bool SetVisibility
    {
        get
        {
            return this.gameObject.activeSelf;
        }
        set
        {
            this.gameObject.SetActive(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // RegisterEvent
        StateManager.Instance.ItemPickupEvent += this.OnItemPickup;
        StateManager.Instance.UsedItemEvent += this.UsedItem;

        pickedUpItemsSprites = new List<Sprite>();
        amountOfItems = GameObject.FindObjectsOfType<Item>().Length;
        itemCounterText.text = pickedUpItemsCounter + " / " + amountOfItems;

        // Spawn all Health sprites
        healthObjects = UIBuilder.CreateIcons(IconParent.transform, HealthSprite, "HealthIcon", PlayerHealth.MaxHealth, new Vector2(0, 1), new Vector2(0, 1), 0.2f, 15);
    }

    public void UpdateHealthIcons(int playerHealth)
    {
        Debug.Log("Player hit! Player Health: " + playerHealth);

        for (int i = PlayerHealth.MaxHealth - 1; i >= 0; i--)
        {
            if (i >= playerHealth && healthObjects[i] != null)
                Destroy(healthObjects[i]);
        }
    }

    public void OnItemPickup(ItemData item)
    {
        Debug.Log("Picked up Item: " + item.Name);

        UpdateItemCounter();

        pickedUpItemsSprites.Add(item.Icon);
        UpdateItemSprites();
    }

    public void UpdateItemCounter()
    {
        pickedUpItemsCounter++;

        // Update Item Counter
        itemCounterText.text = pickedUpItemsCounter + " / " + amountOfItems;

        // GOT ALL ITEMS
        if (pickedUpItemsCounter == amountOfItems)
        {
            itemCounterText.color = Color.green;
            StateManager.Instance.AllItemsCollectedEvent.Invoke();
        }
    }

    public void DisplayInteractionText(bool active)
    {
        InteractionText.SetActive(active);
    }

    public void UsedItem(ItemData usedItem)
    {
        print("Used Item: " + usedItem);  
        pickedUpItemsSprites.Remove(usedItem.Icon);
        UpdateItemSprites();
    }

    public void UpdateItemSprites()
    {
        if (pickedUpItems != null)
        {
            // Clear all Item Sprites
            foreach (GameObject sprite in pickedUpItems)
                Destroy(sprite);
        }

        pickedUpItems = UIBuilder.CreateIcons(IconParent.transform, pickedUpItemsSprites.ToArray(), "Item Icon", new Vector2(1, 0), new Vector2(1, 0), new Vector2(50, 50));
    }
}
