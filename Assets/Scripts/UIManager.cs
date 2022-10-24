using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite HealthSprite;
    
    public static UIManager Instance { get; private set; }

    private GameObject[] healthObjects;
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
        CanvasRect = this.GetComponent<RectTransform>();
        
        StateManager.Instance.PlayerHit += OnPlayerHit;
        healthObjects = CreateIcons(HealthSprite, "HealthIcon", StateManager.Instance.PlayerHealth, new Vector2(0, 1), new Vector2(0, 1), 0.2f);
    }

    private GameObject[] CreateIcons(Sprite icon, string displayName, int numberOfIcons, Vector2 anchor, Vector2 pivot, float scale)
    {
        GameObject[] iconArray = new GameObject[numberOfIcons];

        for (int i = 0; i < numberOfIcons; i++)
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

            rectTransform.anchoredPosition = new Vector2(i * (rectTransform.rect.width * scale), 0);

            // Add the sprite
            Image image = newIcon.AddComponent<Image>();
            image.sprite = icon;

            iconArray[i] = newIcon;
        }

        return iconArray;
    }

    public void OnPlayerHit()
    {
        Debug.Log("Player hit!");

        // Remove a hearth
        if(StateManager.Instance.PlayerHealth >= 0)
            Destroy(healthObjects[StateManager.Instance.PlayerHealth]);
    }
}
