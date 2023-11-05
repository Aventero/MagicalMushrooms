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
    public bool ShouldHaveOutline = true;

    protected void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        player = GameObject.FindGameObjectWithTag("Player");

        Outline = GetComponent<Outline>();
        if (Outline == null)
            Outline = gameObject.AddComponent<Outline>();

        if (ShouldHaveOutline)
        {
            Outline.enabled = true;
            Outline.OutlineWidth = 1;
            Outline.OutlineMode = Outline.Mode.OutlineAll;
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (CanInteract)
        {
            if (!other.CompareTag("Player"))
                return;

            if (!string.IsNullOrEmpty(InteractionText))
                UIManager.Instance.ShowSimpleTooltip(InteractionText);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!string.IsNullOrEmpty(InteractionText))
            UIManager.Instance.HideTooltip();
    }

    public virtual void Interact()
    {

    }

    public virtual void InPlayerSight()
    {
        UIManager.Instance.ShowSimpleTooltip(InteractionText);

    }
    public virtual void OutOfPlayerSight()
    {
        UIManager.Instance.HideTooltip();
    }

    private void OnDisable()
    {
        UIManager.Instance.HideTooltip();
    }

    private void OnDestroy()
    {
        UIManager.Instance.HideTooltip();
    }
}
