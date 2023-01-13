using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverlayMenu : MonoBehaviour
{
    [SerializeField] private Sprite HealthSprite;
    [SerializeField] private TMP_Text itemCounterText;
    public GameObject InteractionText;

    private GameObject[] healthObjects;
    
    private List<GameObject> pickedUpItems; // List for displaying the item sprites

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

        pickedUpItems = new List<GameObject>();
        amountOfItems = GameObject.FindObjectsOfType<Item>().Length;
        itemCounterText.text = pickedUpItemsCounter + " / " + amountOfItems;

        // Spawn all Health sprites
        healthObjects = UIManager.Instance.CreateIcons(HealthSprite, "HealthIcon", PlayerHealth.MaxHealth, new Vector2(0, 1), new Vector2(0, 1), 0.2f);
    }

    public void AddIcon(Sprite icon, string displayName)
    {
        float width = 100;
        float scale = 0.3f;
        Vector2 position = -new Vector3(pickedUpItems.Count * width * scale, 0, 0);

        GameObject newIcon = UIManager.Instance.CreateSpriteOnScreen(position, icon, displayName, new Vector2(1, 1), new Vector2(1, 1), scale);
        pickedUpItems.Add(newIcon);
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
        //Debug.Log("Picked up Item: " + item.Name + " Counter: " + pickedUpItems.Count.ToString());
        UpdateItemCounter();
        DisplayItemIcons(item.Icon);
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

    public void DisplayItemIcons(Sprite icon)
    {
        UIManager.Instance.CreateIcons(icon, "Item Icon", 1, new Vector2(1, 0), new Vector2(1, 0), 0.2f);
    }
}
