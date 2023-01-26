using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public ItemData item;
    public Color InPlayerSightColor;
    private Color standardColor;
    private MeshRenderer meshRenderer;

    private new void Start()
    {
        base.Start(); // Call parent start method

        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        standardColor = meshRenderer.material.color;
    }

    public override void InPlayerSight()
    {
        if (meshRenderer == null)
            return;

        Debug.Log("IN SIGHT");
        UIManager.Instance.ShowInteractionText(InteractionText);
        Material material = meshRenderer.material;
        material.color = InPlayerSightColor;
    }

    public override void OutOfPlayerSight()
    {
        if (meshRenderer == null)
            return;
        
        UIManager.Instance.HideInteractionText();
        Material material = meshRenderer.material;
        material.color = standardColor;
    }

    public override void Interact()
    {
        // Pickup Item
        StateManager.Instance.ItemPickupEvent(item);

        // Destroy picked up item
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        UIManager.Instance.HideInteractionText();
    }
}