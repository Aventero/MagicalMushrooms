using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Interactable
{
    public string neededItemName;
    public Mesh FullGrownMesh;
    private PlayerInventory inventory;
    public GameObject GrownPlant;

    private new void Start()
    {
        base.Start();
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public override void Interact()
    {
        print("Interacting Plant has Item: " + inventory.HasItem(neededItemName));
        if (!inventory.HasItem(neededItemName))
            return;

        inventory.RemoveItem(neededItemName);
        UIManager.Instance.HideInteractionText();
        GrownPlant.SetActive(true);
        gameObject.SetActive(false);

    }

    public override void InPlayerSight()
    {
        UIManager.Instance.ShowInteractionText(InteractionText);
    }

    public override void OutOfPlayerSight()
    {
        UIManager.Instance.HideInteractionText();
    }

    private void OnDestroy()
    {
        UIManager.Instance.HideInteractionText();
    }
}
