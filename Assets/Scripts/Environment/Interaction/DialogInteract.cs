using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DialogInteract : Interactable
{
    [Header("Dialog")]
    public Dialog conversation;
    public bool ShouldDestoryOnComplete = false;
    public UnityEvent OnDialogComplete;

    [Header("Camera Sequence")]
    public Transform LookAt;
    public CinemachineVirtualCamera scriptedSequenceCam;

    public override void Interact()
    {
        if (!CanInteract)
            return;

        UIManager.Instance.HideTooltip();
        UIManager.Instance.ShowDialog(conversation);
        scriptedSequenceCam.Priority = 20;
        scriptedSequenceCam.LookAt = LookAt;
        StateManager.Instance.EndedDialogEvent.AddListener(EndedDialog);
    }

    public void EndedDialog()
    {
        scriptedSequenceCam.Priority = 0;
        OnDialogComplete.Invoke();
        if (ShouldDestoryOnComplete)
            Destroy(gameObject);
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
