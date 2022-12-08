using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite HealthSprite;
    [SerializeField] private GameObject OverlayParent;
    [SerializeField] private TMP_Text itemCounterText;

    private GameObject[] healthObjects;
    private List<GameObject> pickedUpItems; // List for displaying the item sprites

    private bool hideOverlay;
    private int amountOfItems;
    private int pickedUpItemsCounter = 0;

    public static UIManager Instance { get; private set; }

    public bool SetOverlayVisibility
    {
        get
        {
            return hideOverlay;
        }

        set
        {
            OverlayParent.SetActive(value);
        }
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        SetOverlayVisibility = true;

        pickedUpItems = new List<GameObject>();

        amountOfItems = GameObject.FindObjectsOfType<Item>().Length;
        itemCounterText.text = pickedUpItemsCounter + " / " + amountOfItems;

        // RegisterEvent
        StateManager.Instance.ItemPickupEvent += OnItemPickup;

        // Spawn all Health sprites
        healthObjects = CreateIcons(HealthSprite, "HealthIcon", PlayerHealth.MaxHealth, new Vector2(0, 1), new Vector2(0, 1), 0.2f);
    }

    public void UpdateHealthIcons(int playerHealth)
    {
        Debug.Log("Player hit! Player Health: " + playerHealth);

        for (int i = (PlayerHealth.MaxHealth - 1); i >= 0; i--)
        {
            if (i >= playerHealth && healthObjects[i] != null)
                Destroy(healthObjects[i]);
        }
    }

    public void OnItemPickup(Item item)
    {
        Debug.Log("Picked up Item: " + item.Name + " Counter: " + pickedUpItems.Count.ToString());
        pickedUpItemsCounter++;
        itemCounterText.text = pickedUpItemsCounter + " / " + amountOfItems;

        // GOT ALL ITEMS
        if(pickedUpItemsCounter == amountOfItems)
        {
            itemCounterText.color = Color.green;
            StateManager.Instance.AllItemsCollectedEvent.Invoke();
        }
        
    }

    public GameObject[] CreateIcons(Sprite icon, string displayName, int numberOfIcons, Vector2 anchor, Vector2 pivot, float scale)
    {
        GameObject[] iconArray = new GameObject[numberOfIcons];
        float width = 100;

        for (int i = 0; i < numberOfIcons; i++)
        {
            // TODO: solve the width issue
            //i * (rectTransform.rect.width * scale)
            GameObject newIcon = CreateSpriteOnScreen(new Vector2(i * width * scale, 0), icon, displayName, anchor, pivot, scale);
            
            iconArray[i] = newIcon;
        }

        return iconArray;
    }

    public void AddIcon(Sprite icon, string displayName)
    {
        float width = 100;
        float scale = 0.3f;
        Vector2 position = -new Vector3(pickedUpItems.Count * width * scale, 0, 0);

        GameObject newIcon = CreateSpriteOnScreen(position, icon, displayName, new Vector2(1, 1), new Vector2(1, 1), scale);
        pickedUpItems.Add(newIcon);
    }

    private GameObject CreateSpriteOnScreen(Vector2 position, Sprite icon, string displayName, Vector2 anchor, Vector2 pivot, float scale)
    {
        // Spawn new health icon
        GameObject newIcon = new GameObject(displayName);
        newIcon.transform.parent = OverlayParent.transform;

        // position the icon
        RectTransform rectTransform = newIcon.AddComponent<RectTransform>();

        // Set anchor point and the pivot to the top left corner
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = pivot;
        rectTransform.localScale = new Vector3(scale, scale);

        rectTransform.anchoredPosition = position;

        // Add the sprite
        Image image = newIcon.AddComponent<Image>();
        image.sprite = icon;
        image.color = Color.red;

        return newIcon;
    }
}
