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

    [Header("Interaction")]
    public Image InteractionPopup;
    public TMP_Text InteractionText;

    [Header("UI References")]
    public GameObject Tooltip;
    public GameObject IconParent;
    public GameObject Dialog;
    public GameObject Monolog;
    public GameObject CheckpointText;

    public float CheckpointNotificationDuration = 1.5f;

    private List<Sprite> pickedUpItemsSprites;
    private GameObject[] pickedUpItems; // List for displaying the item sprites

    private GameObject activeSkillObject = null;
    private Color activeSkillColor = Color.white;

    private DialogMenu dialogMenu;
    private MonologMenu monologMenu;

    // Animation
    public Animator mouseAnimator;
    public Image LeftClickImage;
    public Image RightClickImage;
    public ToolTipType activeToolTipType;
    private TMP_Text tooltipText;
    
    public void Init()
    {
        // RegisterEvent
        StateManager.Instance.ItemPickupEvent += this.OnItemPickup;
        StateManager.Instance.UsedItemEvent += this.UsedItem;
        StateManager.Instance.NewCheckpointEvent.AddListener(ShowCheckpointText);

        pickedUpItemsSprites = new List<Sprite>();

        dialogMenu = Dialog.GetComponent<DialogMenu>();
        monologMenu = Monolog.GetComponent<MonologMenu>();
        tooltipText = Tooltip.GetComponentInChildren<TMP_Text>(true);

        HideTooltip();
    }

    public void ShowCheckpointText()
    {
        CheckpointText.SetActive(true);
        StartCoroutine(HideCheckpointTextAfterSeconds(CheckpointNotificationDuration));
    }

    private IEnumerator HideCheckpointTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CheckpointText.SetActive(false);
    }

    public void ShowDialog(Dialog conversation)
    {
        Dialog.SetActive(true);
        dialogMenu.ShowDialog(conversation);
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

        activeToolTipType = type;
        tooltipText.SetText(text);
        Tooltip.SetActive(true);

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
    }

    public void ShowTooltip(string text, ToolTipType type)
    {
        if (activeToolTipType == ToolTipType.Skill)
            return;

        activeToolTipType = type;
        tooltipText.SetText(text);
        Tooltip.SetActive(true);
        LeftClickImage.enabled = false;
        RightClickImage.enabled = false;
    }

    public void HideTooltip()
    {
        activeToolTipType = ToolTipType.None;
        Tooltip.SetActive(false);
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

    public void EnableSkill(PlayerSkill playerSkill)
    {
        SetSkillOpacity(1.0f, playerSkill, true);
    }

    public void DisableSkill(PlayerSkill playerSkill)
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

    public void DisplayInteractionText(bool active, string text)
    {
        InteractionPopup.gameObject.SetActive(active);
        InteractionText.gameObject.SetActive(active);
        InteractionText.SetText(text);
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
            _ => null,
        } ;
    }

    private GameObject GetPlayerSkillLetter(PlayerSkill playerSkill)
    {
        return playerSkill switch
        {
            SmokeBomb => SmokeBombLetter,
            Poltergeist => PoltergeistLetter,
            _ => null,
        };
    }
}

public enum MouseSide
{
    LeftClick,
    RightClick
}

public enum ToolTipType
{
    Skill,
    Charge,
    None
}