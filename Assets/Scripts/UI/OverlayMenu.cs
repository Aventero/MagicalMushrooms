using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMenu : MonoBehaviour
{
    public Sprite HealthSprite;
    public Sprite EmptyHealthSprite;
    public TMP_Text itemCounterText;

    public Image InteractionPopup;
    public TMP_Text InteractionText;
    public GameObject IconParent;
    public GameObject Dialog;
    public GameObject Monolog;
    public GameObject CheckpointText;

    public float ShowCheckpointNotification = 1.5f;

    private GameObject[] healthObjects;

    private List<Sprite> pickedUpItemsSprites;
    private GameObject[] pickedUpItems; // List for displaying the item sprites

    private int amountOfItems;
    private int pickedUpItemsCounter = 0;

    private DialogMenu dialogMenu;
    private MonologMenu monologMenu;

    // Start is called before the first frame update
    void Start()
    {
        // RegisterEvent
        StateManager.Instance.ItemPickupEvent += this.OnItemPickup;
        StateManager.Instance.UsedItemEvent += this.UsedItem;
        StateManager.Instance.RespawnPlayerEvent.AddListener(FillPlayerHealth);
        StateManager.Instance.NewCheckpointEvent.AddListener(ShowCheckpointText);

        pickedUpItemsSprites = new List<Sprite>();
        amountOfItems = GameObject.FindObjectsOfType<Item>().Length;
        itemCounterText.text = pickedUpItemsCounter + " / " + amountOfItems;

        dialogMenu = Dialog.GetComponent<DialogMenu>();
        monologMenu = Monolog.GetComponent<MonologMenu>();

        // Spawn all Health sprites
        healthObjects = UIBuilder.CreateIcons(IconParent.transform, HealthSprite, "HealthIcon", PlayerHealth.MaxHealth, new Vector2(0.0f, 0.99f), new Vector2(0.0f, 0.99f), new Vector2(50, 50), 1, 5);
    }

    public void FillPlayerHealth()
    {
        foreach (GameObject healthObject in healthObjects)
            healthObject.GetComponent<Image>().sprite = HealthSprite;
    }

    public void ShowCheckpointText()
    {
        CheckpointText.SetActive(true);
        StartCoroutine(HideCheckpointTextAfterSeconds(ShowCheckpointNotification));
    }

    private IEnumerator HideCheckpointTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CheckpointText.SetActive(false);
    }

    public void ShowDialog(Dialog conversation)
    {
        Dialog.SetActive(true);
        dialogMenu.ShowDialog(conversation);
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        Monolog.SetActive(true);
        monologMenu.ShowMonolog(monolog, target);
    }

    public void ShowMonolog(Monolog monolog)
    {
        Monolog.SetActive(true);
        monologMenu.ShowMonolog(monolog);
    }

    public void UpdateHealthIcons(int playerHealth)
    {
        Debug.Log("Player hit! Player Health: " + playerHealth);

        for (int i = PlayerHealth.MaxHealth - 1; i >= 0; i--)
        {
            if (i >= playerHealth && healthObjects[i] != null)
                healthObjects[i].GetComponent<Image>().sprite = EmptyHealthSprite;
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

    public void DisplayInteractionText(bool active, string text)
    {
        InteractionPopup.gameObject.SetActive(active);
        InteractionText.gameObject.SetActive(active);
        InteractionText.SetText(text);
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
