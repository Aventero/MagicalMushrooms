using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : Interactable
{
    public Dialog conversation;
    public bool InteractionNecessary = false;
    public bool DestroyIfDone = false;

    public override void InPlayerSight()
    {
        if (InteractionNecessary)
            return;

        ShowDialog();
    }

    public override void Interact()
    {
        ShowDialog();
    }

    public override void OutOfPlayerSight() { }

    public void ShowDialog()
    {
        // TODO: Remove the Line
        MonologManager.Instance.DisplayMonolog(WitchType.ANGRY);

        UIManager.Instance.HideInteractionText();
        UIManager.Instance.ShowDialog(conversation);

        if(DestroyIfDone)
            Destroy(this.gameObject);
    }

}
