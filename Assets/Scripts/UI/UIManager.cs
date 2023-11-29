using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
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
    public string[] fallenTexts;
    public string[] catchedTexts;

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

        StateManager.Instance.PlayerHasFallenEvent += this.PlayerHasFallen;
        StateManager.Instance.PlayerWasCaughtEvent += this.PlayerWasCaught;

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

    private void PlayerHasFallen()
    {
        AudioManager.Instance.Play("playerFell");
        PlayerDiedMenu.SetActive(true);
        StateManager.Instance.IsAllowedToSeePlayer = false;
        CanvasGroup canvasGroup = PlayerDiedMenu.GetComponent<CanvasGroup>();
        string text = "<i>" + fallenTexts[Random.Range(0, fallenTexts.Length)] + "</i>\n"
            + "<b><color=#FFFFFF>You dropped <color=#DD2B2B>" + Stats.Instance.CoinsCollected / 2 + "</color> magic.</color></b>";
        Stats.Instance.DecreaseCoinsCollected(Stats.Instance.CoinsCollected / 2);
        PlayerDiedMenu.GetComponentInChildren<TMP_Text>().text = text;
        StartCoroutine(FadeCanvasGroup(1f, canvasGroup));

    }

    private void PlayerWasCaught()
    {
        PlayerDiedMenu.SetActive(true);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CharacterController controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        StateManager.Instance.PausePlayerMovementEvent.Invoke();
        CanvasGroup canvasGroup = PlayerDiedMenu.GetComponent<CanvasGroup>();
        string text = "<i>" + catchedTexts[Random.Range(0, catchedTexts.Length)] + "</i>\n"
            + "<b><color=#FFFFFF>She took <color=#DD2B2B>" + Stats.Instance.CoinsCollected / 2 + "</color> magic.</color></b>";
        Stats.Instance.DecreaseCoinsCollected(Stats.Instance.CoinsCollected / 2);
        PlayerDiedMenu.GetComponentInChildren<TMP_Text>().text = text;
        StartCoroutine(FadeCanvasGroup(1f, canvasGroup));
    }

    private IEnumerator FadeCanvasGroup(float time, CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / time;
            yield return null;
        }

        StartCoroutine(FadeBackToNormalAfter(3f, 0.5f, canvasGroup));
    }

    private IEnumerator FadeBackToNormalAfter(float waitTime, float fadeTime, CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(FadeBackToNormal(fadeTime, canvasGroup));
    }

    private IEnumerator FadeBackToNormal(float time, CanvasGroup canvasGroup)
    {
        CheckpointManager.Instance.RespawnPlayer();
        StateManager.Instance.ResumePlayerMovementEvent.Invoke();
        OverlayMenu.SetActive(true);
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / time;
            yield return null;
        }
        PlayerDiedMenu.SetActive(false);
        StateManager.Instance.IsAllowedToSeePlayer = true;
    }
}
