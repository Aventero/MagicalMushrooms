using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private GameObject OverlayMenu;
    [SerializeField] private GameObject GameOverMenu;

    public static UIManager Instance { get; private set; }

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
        OverlayMenu.SetActive(true);
        GameOverMenu.SetActive(false);

        StateManager.Instance.GameOverEvent += this.GameOver;
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

    public GameObject CreateSpriteOnScreen(Vector2 position, Sprite icon, string displayName, Vector2 anchor, Vector2 pivot, float scale)
    {
        // Spawn new health icon
        GameObject newIcon = new GameObject(displayName);
        newIcon.transform.parent = OverlayMenu.transform;

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

    public void UpdateHealthIcons(int playerHealth)
    {
        OverlayMenu.GetComponent<OverlayMenu>().UpdateHealthIcons(playerHealth);
    }

    public void PauseGame()
    {
        StateManager.Instance.PauseGameEvent.Invoke();
    }

    public void ResumeGame()
    {
        StateManager.Instance.ResumeGameEvent.Invoke();
    }

    public void ShowInteractionText(bool active)
    {
        if (OverlayMenu != null)
            OverlayMenu.GetComponent<OverlayMenu>().DisplayInteractionText(active);

    }

    private void GameOver()
    {
        PauseGame();
        GameOverMenu.SetActive(true);
    }
}
