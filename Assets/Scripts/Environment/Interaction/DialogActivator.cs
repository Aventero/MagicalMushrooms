using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DialogActivator : MonoBehaviour
{
    [Header("Dialog")]
    public Dialog conversation;
    public bool CanInteractWith = false;
    public bool ShouldDestoryOnComplete = false;
    public UnityEvent OnDialogComplete;

    [Header("Camera Sequence")]
    public Transform LookAt;
    public CinemachineVirtualCamera scriptedSequenceCam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowDialog();
        }
    }

    public void ShowDialog()
    {
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
}
