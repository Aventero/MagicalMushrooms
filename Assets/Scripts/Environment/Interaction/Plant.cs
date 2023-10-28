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
        UIManager.Instance.HideTooltip();
        GrownPlant.SetActive(true);
        gameObject.SetActive(false);

    }

    public override void InPlayerSight()
    {
        UIManager.Instance.ShowSimpleTooltip(InteractionText);
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
