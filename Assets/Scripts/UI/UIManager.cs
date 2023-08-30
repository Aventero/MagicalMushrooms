using UnityEngine;

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

    public void HideInteractionText()
    {
        if (OverlayMenu != null)
            OverlayMenu.GetComponent<OverlayMenu>().DisplayInteractionText(false, "");
    }

    public void ShowInteractionText(string text)
    {
        if (OverlayMenu != null)
            OverlayMenu.GetComponent<OverlayMenu>().DisplayInteractionText(true, text);
    }

    public void ShowDialog(Dialog conversation)
    {
        OverlayMenu.GetComponent<OverlayMenu>().ShowDialog(conversation);
    }

    private void GameOver()
    {
        PauseGame();
        GameOverMenu.SetActive(true);
    }
}
