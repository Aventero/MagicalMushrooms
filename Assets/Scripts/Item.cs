using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public string Name;
    public Sprite Icon;

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

        Material material = meshRenderer.material;
        material.color = InPlayerSightColor;
    }

    public override void OutOfPlayerSight()
    {
        if (meshRenderer == null)
            return;

        Material material = meshRenderer.material;
        material.color = standardColor;
    }

    public override void Interact()
    {
        // Pickup Item
        Item item = this.GetComponent<Item>();
        StateManager.Instance.ItemPickupEvent(item);

        // Destroy picked up item
        Destroy(this.gameObject);
    }
}