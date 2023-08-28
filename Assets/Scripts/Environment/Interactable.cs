using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [Header("Sprites:")]
    public Sprite InRangeSprite;
    public Image Image;
    public Outline Outline;
    public string InteractionText;
    private GameObject player;

    protected void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        player = GameObject.FindGameObjectWithTag("Player");
        Outline = GetComponent<Outline>();
        Outline.enabled = true;
        Outline.OutlineWidth = 1;
        Outline.OutlineMode = Outline.Mode.OutlineAll;

        if (Image)
        {
            Image.sprite = InRangeSprite;
            Image.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        if(Image)
            Image.transform.LookAt(player.transform);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!string.IsNullOrEmpty(InteractionText))
            UIManager.Instance.ShowInteractionText(InteractionText);
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

    public abstract void Interact();
    public abstract void InPlayerSight();
    public abstract void OutOfPlayerSight();
}
