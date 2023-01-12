using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Interactable
{
    public string neededItemName;
    public Mesh FullGrownMesh;
    private PlayerInventory inventory;

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

        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        MeshCollider meshCollider = this.gameObject.GetComponent<MeshCollider>();

        meshFilter.mesh = FullGrownMesh;
        meshCollider.sharedMesh = FullGrownMesh;

        inventory.RemoveItem(neededItemName);
    }

    public override void InPlayerSight()
    {

    }

    public override void OutOfPlayerSight()
    {
        
    }
}
