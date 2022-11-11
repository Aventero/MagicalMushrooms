using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite HealthSprite;
    
    public static UIManager Instance { get; private set; }

    private GameObject[] healthObjects;
    private List<GameObject> pickedUpItems;

    private RectTransform CanvasRect;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        pickedUpItems = new List<GameObject>();
        CanvasRect = this.GetComponent<RectTransform>();
        
        StateManager.Instance.PlayerHit += OnPlayerHit;
        healthObjects = CreateIcons(HealthSprite, "HealthIcon", StateManager.Instance.PlayerHealth, new Vector2(0, 1), new Vector2(0, 1), 0.2f);
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

    public void OnPlayerHit()
    {
        Debug.Log("Player hit!");

        // Remove a hearts
        if(StateManager.Instance.PlayerHealth >= 0)
            Destroy(healthObjects[StateManager.Instance.PlayerHealth - 1]);
    }

    private GameObject CreateSpriteOnScreen(Vector2 position, Sprite icon, string displayName, Vector2 anchor, Vector2 pivot, float scale)
    {
        // Spawn new health icon
        GameObject newIcon = new GameObject(displayName);
        newIcon.transform.parent = this.transform;

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
