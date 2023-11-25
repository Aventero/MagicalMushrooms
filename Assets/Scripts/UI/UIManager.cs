using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class UIManager : MonoBehaviour
{
    [Header("Menues")]
    [SerializeField] private GameObject OverlayMenu;
    [SerializeField] private GameObject PlayerDiedMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject DialogMenu;

    private OverlayMenu overlayMenu;
    private DialogMenu dialogMenu;

    public static UIManager Instance { get; private set; }
    public bool IsCutscene = false;
    public string SkippingCutsceneName;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        overlayMenu = OverlayMenu.GetComponent<OverlayMenu>();
        overlayMenu.Init();

        OverlayMenu.SetActive(true);
        PlayerDiedMenu.SetActive(false);
        PauseMenu.SetActive(false);
        DialogMenu.SetActive(false);

        StateManager.Instance.PlayerDiedEvent += this.PlayerDied;

        dialogMenu = DialogMenu.GetComponent<DialogMenu>();

        if (IsCutscene)
        {
            OverlayMenu.SetActive(false);
            PlayerDiedMenu.SetActive(false);
            PauseMenu.SetActive(false);
            DialogMenu.SetActive(false);
        }

        Canvas.ForceUpdateCanvases();
    }

    // User hit the escape key 
    public void Escape(CallbackContext context)
    {
        if (!context.performed)
            return;

        if (dialogMenu.isActiveAndEnabled)
            return;

        OverlayMenu.SetActive(false);
        DialogMenu.SetActive(false);
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
        if (overlayMenu == null)
            return;

        overlayMenu.DisplayInteractionText(false, "");
    }

    public void ShowInteractionText(string text)
    {
        if (overlayMenu == null)
            return;
        overlayMenu.DisplayInteractionText(true, text);
    }

    public void ShowSkillTooltip(string text, MouseSide mouseSide)
    {
        if (overlayMenu == null)
            return;
        overlayMenu.ShowTooltip(text, mouseSide, ToolTipType.Skill);
    }

    public void ShowChargeTooltip(string text, MouseSide mouseSide)
    {
        if (overlayMenu == null)
            return;
        overlayMenu.ShowTooltip(text, mouseSide, ToolTipType.Charge);
    }

    public void ShowInteractionTooltip(string text)
    {
        if (overlayMenu == null)
            return;
        overlayMenu.ShowInteractionTooltip(text);
    }

    public void ShowSimpleTextTooltip(string text)
    {
        if (overlayMenu == null)
            return;
        overlayMenu.ShowSimpleTextTooltip(text);
    }

    public void HideTooltip()
    {
        if (overlayMenu == null)
            return;
        overlayMenu.HideTooltip();
    }

    public ToolTipType GetActiveToolTipType()
    {
        return overlayMenu.activeToolTipType;
    }

    public void ShowDialog(Dialog dialog)
    {
        DialogMenu.SetActive(true);
        dialogMenu.ShowDialog(dialog);
        SetOverlayVisibility(false);
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        overlayMenu.ShowMonolog(monolog, target);
    }

    public void EnableSkillVisual(PlayerSkill skill) => overlayMenu.EnableSkillVisually(skill);
    public void DisableSkillVisual(PlayerSkill skill) => overlayMenu.DisableSkillVisually(skill);

    public void SkillActivated(PlayerSkill playerSkill) => overlayMenu.SkillActivated(playerSkill);
    public void SkillDeactivated() => overlayMenu.SkillDeactivated();
    
    public void SkillExecuted(PlayerSkill playerSkill) => overlayMenu.SkillExecuted(playerSkill);

    public void ShowMonolog(Monolog monolog)
    => overlayMenu.ShowMonolog(monolog);

    public void SetSkillBarVisibility(bool visible)
    {
        if (!IsCutscene)
            overlayMenu.SetSkillBarVisibility(visible);
    }

    public void SetOverlayVisibility(bool visible)
    {
        if (!IsCutscene)
            OverlayMenu.SetActive(visible);
    } 
    

    public void ShowSmokeFrame(bool visible)
    {
        overlayMenu.ShowSmokeFrame(visible);
    }

    private void PlayerDied()
    {
        OverlayMenu.SetActive(false);
        PlayerDiedMenu.SetActive(true);

        StartCoroutine(FadePlayerDied());
    }

    IEnumerator FadePlayerDied()
    {
        yield return new WaitForSeconds(1);

        TMP_Text text = PlayerDiedMenu.GetComponentInChildren<TMP_Text>();
        Color color = text.color;

        while (color.a > 0)
        {
            color.a -= Time.deltaTime / 2f;
            text.color = color;
            Debug.Log(color.a);
            yield return null;
        }

        color.a = 1;
        text.color = color;

        OverlayMenu.SetActive(true);
        PlayerDiedMenu.SetActive(false);
    }
}
