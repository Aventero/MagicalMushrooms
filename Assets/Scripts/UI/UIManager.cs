using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private GameObject OverlayMenu;
    [SerializeField] private GameObject GameOverMenu;

    private OverlayMenu overlayMenu;

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

        overlayMenu = OverlayMenu.GetComponent<OverlayMenu>();
    }

    public void UpdateHealthIcons(int playerHealth)
    {
        overlayMenu.UpdateHealthIcons(playerHealth);
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
            overlayMenu.DisplayInteractionText(false, "");
    }

    public void ShowInteractionText(string text)
    {
        if (OverlayMenu != null)
            overlayMenu.DisplayInteractionText(true, text);
    }

    public void ShowDialog(Dialog dialog)
    {
        overlayMenu.ShowDialog(dialog);
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        overlayMenu.ShowMonolog(monolog, target);
    }

    public void ShowMonolog(Monolog monolog)
    {
        overlayMenu.ShowMonolog(monolog);
    }

    private void GameOver()
    {
        PauseGame();
        GameOverMenu.SetActive(true);
    }
}
