using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite HealthSprite;
    
    public static UIManager Instance { get; private set; }

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
        CreateHealthIcons();
    }

    void Update()
    {
        
    }

    private void CreateHealthIcons()
    {
        GameObject healthIcon = new GameObject();
        healthIcon.transform.parent = this.transform;
        RectTransform rectTransform= healthIcon.AddComponent<RectTransform>();
        Image renderer = healthIcon.AddComponent<Image>();
        renderer.sprite = HealthSprite;

        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
    }

    public void OnPlayerHit()
    {
        Debug.Log("Player hit!");
    }
}
