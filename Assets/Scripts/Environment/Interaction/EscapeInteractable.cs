using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EscapeInteractable : Interactable
{
    public override void Interact()
    {
        if (!CanInteract)
            return;
        SceneLoader.Instance.LoadScene("MainMenu");
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
