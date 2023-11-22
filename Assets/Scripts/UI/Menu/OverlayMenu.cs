using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMenu : MonoBehaviour
{
    [Header("Player Skill References")]
    public float DeactivatedSkillOpacity = 0.05f;
    public Color SkillActivationColor;

    public GameObject Skillbar;
    public GameObject PoltergeistObject;
    public GameObject PoltergeistLetter;
    public GameObject SmokeBombObject;
    public GameObject SmokeBombLetter;
    public GameObject DraggingObject;
    public GameObject DraggingLetter;
    public GameObject InteractRing;
    public GameObject SmokeFrame;

    [Header("Interaction")]
    public Image InteractionPopup;
    public TMP_Text InteractionText;

    [Header("UI References")]
    public GameObject Tooltip;
    public GameObject IconParent;
    public GameObject Monolog;
    public GameObject CheckpointText;

    public float CheckpointNotificationDuration = 1.5f;

    private List<Sprite> pickedUpItemsSprites;
    private GameObject[] pickedUpItems; // List for displaying the item sprites

    private GameObject activeSkillObject = null;
    private Color activeSkillColor = Color.white;

    private MonologMenu monologMenu;

    // Animation
    public Animator mouseAnimator;
    public Image Mouse;
    public Image LeftClickImage;
    public Image RightClickImage;
    public ToolTipType activeToolTipType;
    private TMP_Text tooltipText;
    public static OverlayMenu Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        // RegisterEvent
        StateManager.Instance.ItemPickupEvent += this.OnItemPickup;
        StateManager.Instance.UsedItemEvent += this.UsedItem;
        StateManager.Instance.NewCheckpointEvent.AddListener(ShowCheckpointText);
        InteractRing.SetActive(false);
        pickedUpItemsSprites = new List<Sprite>();
        monologMenu = Monolog.GetComponent<MonologMenu>();
        tooltipText = Tooltip.GetComponentInChildren<TMP_Text>(true);

        HideTooltip();
    }

    private void OnEnable()
    {
        if (CheckpointText.activeSelf)
            StartCoroutine(FadeTextToZeroAlpha(CheckpointNotificationDuration, CheckpointText.GetComponentInParent<CanvasGroup>()));
    }

    // Call this method to show the checkpoint text
    public void ShowCheckpointText()
    {
        CheckpointText.SetActive(true);
        StartCoroutine(FadeTextToFullAlpha(CheckpointNotificationDuration, CheckpointText.GetComponentInParent<CanvasGroup>()));
    }

    private IEnumerator FadeTextToFullAlpha(float time, CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / time;
            yield return null;
        }

        StartCoroutine(HideCheckpointTextAfterSeconds(CheckpointNotificationDuration, canvasGroup));
    }

    private IEnumerator HideCheckpointTextAfterSeconds(float seconds, CanvasGroup canvasGroup)
    {
        yield return new WaitForSecondsRealtime(seconds);
        StartCoroutine(FadeTextToZeroAlpha(seconds, canvasGroup));
    }

    private IEnumerator FadeTextToZeroAlpha(float time, CanvasGroup canvasGroup)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / time;
            yield return null;
        }

        CheckpointText.SetActive(false);
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        Monolog.SetActive(true);
        monologMenu.ShowMonolog(monolog, target);
    }

    public void ShowMonolog(Monolog monolog)
    {
        Monolog.SetActive(true);
        monologMenu.ShowMonolog(monolog);
    }

    public void ShowTooltip(string text, MouseSide mouseButton, ToolTipType type)
    {
        if (activeToolTipType == ToolTipType.Skill)
            return;

        Mouse.enabled = true;
        activeToolTipType = type;
        tooltipText.SetText(text);
        Tooltip.SetActive(true);
        InteractRing.SetActive(true);

        if (mouseButton == MouseSide.LeftClick)
        {
            LeftClickImage.enabled = true;
            RightClickImage.enabled = false;
            mouseAnimator.Play("LeftClick");
        }

        if (mouseButton == MouseSide.RightClick)
        {
            LeftClickImage.enabled = false;
            RightClickImage.enabled = true;
            mouseAnimator.Play("RightClick");
        }

        if (mouseButton == MouseSide.None)
        {
            LeftClickImage.enabled = false;
            RightClickImage.enabled = false;
            Mouse.enabled = false;
            InteractRing.SetActive(false);
        }
    }

    public void ShowInteractionTooltip(string text)
    {
        if (activeToolTipType == ToolTipType.Skill)
            return;

        activeToolTipType = ToolTipType.None;
        tooltipText.SetText("<color=yellow>[E]</color> - " + text);
        Tooltip.SetActive(true);
        InteractRing.SetActive(true);
        LeftClickImage.enabled = false;
        RightClickImage.enabled = false;
        Mouse.enabled = false;
    }

    public void ShowSimpleTextTooltip(string text)
    {
        if (activeToolTipType == ToolTipType.Skill)
            return;

        activeToolTipType = ToolTipType.None;
        tooltipText.SetText(text);
        Tooltip.SetActive(true);
        InteractRing.SetActive(false);
        LeftClickImage.enabled = false;
        RightClickImage.enabled = false;
        Mouse.enabled = false;
    }

    public void HideTooltip()
    {
        activeToolTipType = ToolTipType.None;
        Tooltip.SetActive(false);
        InteractRing.SetActive(false);
    }

    private void SetSkillOpacity(float opacity, PlayerSkill playerSkill, bool turnOnSkillLetter)
    {
        GameObject skillObject = GetPlayerSkillObject(playerSkill);

        Color skillColor = activeSkillColor;
        skillColor.a = opacity;

        GameObject skillLetter = GetPlayerSkillLetter(playerSkill);
        skillLetter.SetActive(turnOnSkillLetter);

        CutoutMask[] masks = skillObject.GetComponentsInChildren<CutoutMask>();
        foreach (CutoutMask mask in masks)
            mask.color = skillColor;

        Image[] images = skillObject.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            Color color = image.color;
            color.a = opacity;

            image.color = color;
        }
    }

    public void EnableSkillVisually(PlayerSkill playerSkill)
    {
        SetSkillOpacity(1.0f, playerSkill, true);
    }

    public void DisableSkillVisually(PlayerSkill playerSkill)
    {
        SetSkillOpacity(DeactivatedSkillOpacity, playerSkill, false);
    }

    public void SkillActivated(PlayerSkill playerSkill)
    {
        activeSkillObject = GetPlayerSkillObject(playerSkill);

        CutoutMask mask = activeSkillObject.GetComponentInChildren<CutoutMask>();

        activeSkillColor = mask.color;
        mask.color = SkillActivationColor;
    }

    public void SkillDeactivated()
    {
        if (activeSkillObject == null)
            return;

        activeSkillObject.GetComponentInChildren<CutoutMask>().color = activeSkillColor;
        activeSkillObject = null;
    }

    public void SkillExecuted(PlayerSkill playerSkill)
    {
        SkillDeactivated();

        SkillRecharger skill = GetPlayerSkillObject(playerSkill).GetComponent<SkillRecharger>();
        skill.ChargeSkill(playerSkill.RechargeTime);
    }

    public void SetSkillBarVisibility(bool visibility)
    {
        Skillbar.SetActive(visibility);
    }

    public void OnItemPickup(ItemData item)
    {
        Debug.Log("Picked up Item: " + item.Name);

        pickedUpItemsSprites.Add(item.Icon);
        UpdateItemSprites();
    }

    public void ShowSmokeFrame(bool visible)
    {
        if (SmokeFrame.TryGetComponent<SmokeFrame>(out var smokeFrame))
        {
            if (visible)
                smokeFrame.Reveal();
            else
                smokeFrame.Hide();
        }
    }

    public void DisplayInteractionText(bool active, string text)
    {
        InteractionPopup.gameObject.SetActive(active);
        InteractionText.gameObject.SetActive(active);
        InteractionText.SetText(text);
        InteractRing.SetActive(active);
    }

    public void UsedItem(ItemData usedItem)
    {
        print("Used Item: " + usedItem);  
        pickedUpItemsSprites.Remove(usedItem.Icon);
        UpdateItemSprites();
    }

    public void UpdateItemSprites()
    {
        if (pickedUpItems != null)
        {
            // Clear all Item Sprites
            foreach (GameObject sprite in pickedUpItems)
                Destroy(sprite);
        }

        pickedUpItems = UIBuilder.CreateIcons(IconParent.transform, pickedUpItemsSprites.ToArray(), "Item Icon", new Vector2(1, 0), new Vector2(1, 0), new Vector2(50, 50));
    }

    private GameObject GetPlayerSkillObject(PlayerSkill playerSkill)
    {
        return playerSkill switch
        {
            SmokeBomb => SmokeBombObject,
            Poltergeist => PoltergeistObject,
            Dragging => DraggingObject,
            _ => null,
        } ;
    }

    private GameObject GetPlayerSkillLetter(PlayerSkill playerSkill)
    {
        return playerSkill switch
        {
            SmokeBomb => SmokeBombLetter,
            Poltergeist => PoltergeistLetter,
            Dragging => DraggingLetter,
            _ => null,
        };
    }
}

public enum MouseSide
{
    LeftClick,
    RightClick,
    None
}

public enum ToolTipType
{
    Skill,
    Charge,
    None
}