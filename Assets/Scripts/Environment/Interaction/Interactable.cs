using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    public Outline Outline;
    public string InteractionText;
    protected GameObject player;
    public bool CanInteract = true;

    protected void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        player = GameObject.FindGameObjectWithTag("Player");
        Outline = GetComponent<Outline>();
        Outline.enabled = true;
        Outline.OutlineWidth = 1;
        Outline.OutlineMode = Outline.Mode.OutlineAll;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (CanInteract)
        {
            if (!other.CompareTag("Player"))
                return;

            if (!string.IsNullOrEmpty(InteractionText))
                UIManager.Instance.ShowInteractionText(InteractionText);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!string.IsNullOrEmpty(InteractionText))
            UIManager.Instance.HideInteractionText();
    }

    public virtual void Interact()
    {

    }

    public virtual void InPlayerSight()
    {
        UIManager.Instance.ShowInteractionText(InteractionText);

    }
    public virtual void OutOfPlayerSight()
    {
        UIManager.Instance.HideInteractionText();
    }

    private void OnDisable()
    {
        UIManager.Instance.HideInteractionText();
    }

    private void OnDestroy()
    {
        UIManager.Instance.HideInteractionText();
    }
}
