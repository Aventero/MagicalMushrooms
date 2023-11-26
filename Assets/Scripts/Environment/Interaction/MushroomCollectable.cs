using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MushroomCollectable : Interactable
{
    public override void Interact()
    {
        if (!CanInteract)
            return;

        Stats.Instance.IncreaseMushroomsCollected(this.transform.position);
        Destroy(transform.parent.gameObject);
        QuestManager.Instance.RemoveQuest(GetComponent<Quest>());
    }

    public void Collect()
    {
        Stats.Instance.IncreaseMushroomsCollected(this.transform.position);
        Destroy(transform.parent.gameObject);
        QuestManager.Instance.RemoveQuest(GetComponent<Quest>());
    }

    public override void InPlayerSight()
    {
        UIManager.Instance.ShowInteractionTooltip(InteractionText);
    }

    public override void OutOfPlayerSight()
    {
        UIManager.Instance.HideTooltip();
    }

    private void OnDestroy()
    {
        UIManager.Instance.HideTooltip();
    }
}
