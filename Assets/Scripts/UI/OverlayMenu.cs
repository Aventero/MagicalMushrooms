using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMenu : MonoBehaviour
{
    public Image InteractionPopup;
    public TMP_Text TooltipText;
    public TMP_Text InteractionText;

    public GameObject IconParent;
    public GameObject Dialog;
    public GameObject Monolog;
    public GameObject CheckpointText;

    public GameObject Skillbar;
    public GameObject PoltergeistObject;
    public GameObject SmokeBombObject;
    public Color SkillActivationColor;

    public float ShowCheckpointNotification = 1.5f;

    private List<Sprite> pickedUpItemsSprites;
    private GameObject[] pickedUpItems; // List for displaying the item sprites

    private DialogMenu dialogMenu;
    private MonologMenu monologMenu;
    private GameObject activeSkillObject;
    private Color activeSkillColor;

    // Start is called before the first frame update
    void Start()
    {
        // RegisterEvent
        StateManager.Instance.ItemPickupEvent += this.OnItemPickup;
        StateManager.Instance.UsedItemEvent += this.UsedItem;
        StateManager.Instance.NewCheckpointEvent.AddListener(ShowCheckpointText);

        pickedUpItemsSprites = new List<Sprite>();

        dialogMenu = Dialog.GetComponent<DialogMenu>();
        monologMenu = Monolog.GetComponent<MonologMenu>();

        HideTooltip();
    }

    public void ShowCheckpointText()
    {
        CheckpointText.SetActive(true);
        StartCoroutine(HideCheckpointTextAfterSeconds(ShowCheckpointNotification));
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

    public void SetSkillBarVisibility(bool visibility)
    {
        Skillbar.SetActive(visibility);
    }

    public void ShowTooltip(string tooltipText)
    {
        TooltipText.SetText(tooltipText);
        TooltipText.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        TooltipText.SetText(string.Empty);
        TooltipText.gameObject.SetActive(false);
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

    public void SkillActivated(PlayerSkill playerSkill)
    {
        GameObject playerSkillObject = GetPlayerSkillObject(playerSkill);
        CutoutMask mask = playerSkillObject.GetComponentInChildren<CutoutMask>();
        activeSkillColor = mask.color;
        mask.color = SkillActivationColor;
        activeSkillObject = playerSkillObject;
    }

    public void SkillDeactivated()
    {
        CutoutMask mask = activeSkillObject.GetComponentInChildren<CutoutMask>();
        mask.color = activeSkillColor;

        activeSkillObject = null;
        activeSkillColor = Color.white;
    }

    public void SkillExecuted(PlayerSkill playerSkill)
    {
        SkillDeactivated();
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
        };
    }
}
