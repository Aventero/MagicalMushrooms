using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class UIManager : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private GameObject OverlayMenu;
    [SerializeField] private GameObject GameOverMenu;
    [SerializeField] private GameObject PauseMenu;

    private OverlayMenu overlayMenu;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        OverlayMenu.SetActive(true);
        GameOverMenu.SetActive(false);
        PauseMenu.SetActive(false);

        StateManager.Instance.GameOverEvent += this.GameOver;

        overlayMenu = OverlayMenu.GetComponent<OverlayMenu>();
        overlayMenu.Init();
    }

    // User hit the escape key 
    public void Escape(CallbackContext context)
    {
        if (!context.performed)
            return;

        OverlayMenu.SetActive(false);
        PauseMenu.SetActive(true);
        PauseMenu.GetComponent<PauseMenu>().ShowMenu();
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
        overlayMenu.DisplayInteractionText(false, "");
    }

    public void ShowInteractionText(string text)
    {
        overlayMenu.DisplayInteractionText(true, text);
    }

    public void ShowSkillTooltip(string text, MouseSide mouseSide)
    {
        overlayMenu.ShowTooltip(text, mouseSide, ToolTipType.Skill);
    }

    public void ShowChargeTooltip(string text, MouseSide mouseSide)
    {
        overlayMenu.ShowTooltip(text, mouseSide, ToolTipType.Charge);
    }

    public void ShowChargeTooltip(string text)
    {
        overlayMenu.ShowTooltip(text, ToolTipType.Charge);
    }

    public void HideTooltip()
    {
        overlayMenu.HideTooltip();
    }

    public ToolTipType GetActiveToolTipType()
    {
        return overlayMenu.activeToolTipType;
    }

    public void ShowDialog(Dialog dialog)
    {
        overlayMenu.ShowDialog(dialog);
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        overlayMenu.ShowMonolog(monolog, target);
    }

    public void EnableSkill(PlayerSkill skill) => overlayMenu.EnableSkill(skill);
    public void DisableSkill(PlayerSkill skill) => overlayMenu.DisableSkill(skill);

    public void SkillActivated(PlayerSkill playerSkill) => overlayMenu.SkillActivated(playerSkill);
    public void SkillDeactivated() => overlayMenu.SkillDeactivated();
    
    public void SkillExecuted(PlayerSkill playerSkill) => overlayMenu.SkillExecuted(playerSkill);

    public void ShowMonolog(Monolog monolog)
    => overlayMenu.ShowMonolog(monolog);

    public void SetSkillBarVisibility(bool visible) => overlayMenu.SetSkillBarVisibility(visible);

    public void SetOverlayVisibility(bool visible) => OverlayMenu.SetActive(visible);
    

    private void GameOver()
    {
        PauseGame();
        GameOverMenu.SetActive(true);
    }
}
