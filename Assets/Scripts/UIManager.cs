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
        healthObjects = new GameObject[StateManager.Instance.PlayerHealth];
        CanvasRect = this.GetComponent<RectTransform>();
        
        StateManager.Instance.PlayerHit += OnPlayerHit;
        CreateHealthIcons();
    }

    void Update()
    {
        
    }

    private void CreateHealthIcons()
    {
        float scale = 0.2f;
        for(int i = 0; i < StateManager.Instance.PlayerHealth; i++)
        {
            // Spawn new health icon
            GameObject newHealthIcon = new GameObject("HealthIcon");
            newHealthIcon.transform.parent = this.transform;

            // position the icon
            RectTransform rectTransform = newHealthIcon.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.localScale = new Vector3(scale, scale);

            rectTransform.anchoredPosition = new Vector2(i * (rectTransform.rect.width * scale), 0);

            // set the icon sprite
            Image image = newHealthIcon.AddComponent<Image>();
            image.sprite = HealthSprite;

            healthObjects[i] = newHealthIcon;
        }
    }

    public void OnPlayerHit()
    {
        Debug.Log("Player hit!");

        // Remove a hearth
        if(StateManager.Instance.PlayerHealth >= 0)
            Destroy(healthObjects[StateManager.Instance.PlayerHealth]);
    }
}
