using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poltergeist : PlayerSkill
{
    [Header("Values")]
    public float PushForce = 10;
    public float HighlightDistance;

    [Header("References")]
    public Material HighlightMaterial;
    public Material FocusMaterial;

    private readonly List<PoltergeistMovableObject> movableObjectsList = new();
    private bool showHighlighting = false;
    private GameObject lastFocusedObject = null;
    private Camera mainCamera;
    private LayerMask allowedLayers;

    private void Start()
    {
        allowedLayers = LayerMask.GetMask("Default", "Prop");
        mainCamera = Camera.main;
        SetupGameobjects();
    }

    private void SetupGameobjects()
    {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Draggable"))
        {
            PoltergeistMovableObject movableObject = gameObject.AddComponent<PoltergeistMovableObject>();
            movableObject.HighlightMaterial = HighlightMaterial;
            movableObject.HighlightDistance = HighlightDistance;
            movableObject.FocusMaterial = FocusMaterial;
            movableObjectsList.Add(movableObject);

            gameObject.AddComponent<Outline>();
        }
    }

    private void Update()
    {
        if (!showHighlighting)
            return;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 50, allowedLayers))
        {
            CheckCurrentFocusObject(hit);

            if (!hit.collider.CompareTag("Draggable") || hit.distance > HighlightDistance)
                return;

            lastFocusedObject = hit.collider.gameObject;
            lastFocusedObject.GetComponent<PoltergeistMovableObject>().ShowFocus();
        }
        else
            HideCurrentFocusedObject();

    }

    private void CheckCurrentFocusObject(RaycastHit hit)
    {
        if(lastFocusedObject == null) 
            return;

        if (hit.distance > HighlightDistance && hit.collider.gameObject.Equals(lastFocusedObject) )
            HideCurrentFocusedObject(); // The same object was hit and is out of range
        else
            HideCurrentFocusedObject(); // Other gameobject was hit
        
    }

    private void HideCurrentFocusedObject()
    {
        if(lastFocusedObject == null)
            return;

        lastFocusedObject.GetComponent<PoltergeistMovableObject>().HideFocus();
        lastFocusedObject = null;
    }

    public override bool Execute()
    {
        Debug.Log("Execute Poltergeist!");

        if (lastFocusedObject == null)
            return false;

        Rigidbody rigidbody = lastFocusedObject.GetComponent<Rigidbody>();
        rigidbody.AddForce(mainCamera.transform.forward * PushForce, ForceMode.Impulse);

        HidePreview();
        return true;
    }

    public override void ShowPreview()
    {
        IsActivated = true;
        showHighlighting = true;
        
        foreach (PoltergeistMovableObject movableObject in movableObjectsList)
        {
            movableObject.TurnOnHighlighting();
        }
    }

    public override void HidePreview()
    {
        foreach (PoltergeistMovableObject movableObject in movableObjectsList)
        {
            movableObject.TurnOffHighlighting();
        }

        showHighlighting = false;
        IsActivated = false;
    }
}
