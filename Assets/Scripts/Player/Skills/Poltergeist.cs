using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poltergeist : PlayerSkill
{
    public Material HighlightMaterial;
    public Material FocusMaterial;
    public float HighlightDistance;

    private readonly List<PoltergeistMovableObject> movableObjectsList = new();
    private bool showHighlighting = false;
    private GameObject lastFocusedObject = null;
    private GameObject player;
    private Camera mainCamera;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        }
    }

    private void Update()
    {
        if (!showHighlighting)
            return;

        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit))
        {
            CheckCurrentFocusObject(hit);

            if (!hit.collider.CompareTag("Draggable") || hit.distance > HighlightDistance)
                return;

            lastFocusedObject = hit.collider.gameObject;
            lastFocusedObject.GetComponent<PoltergeistMovableObject>().ShowFocus();
        }

    }

    private void CheckCurrentFocusObject(RaycastHit hit)
    {
        if(lastFocusedObject == null) 
            return;

        if (hit.distance > HighlightDistance && hit.collider.gameObject.Equals(lastFocusedObject) )
            HideCurrentFocusedObject(); // The same object was hit
        else
            HideCurrentFocusedObject(); // Other gameobject was hit
        
    }

    private void HideCurrentFocusedObject()
    {
        lastFocusedObject.GetComponent<PoltergeistMovableObject>().HideFocus();
        lastFocusedObject = null;
    }

    public override void Execute()
    {

    }

    public override void ShowPreview()
    {
        IsActivated = true;
        showHighlighting = true;
        
        foreach (PoltergeistMovableObject movableObject in movableObjectsList)
        {
            movableObject.TurnOnHighlight();
        }
    }

    public override void HidePreview()
    {
        foreach (PoltergeistMovableObject movableObject in movableObjectsList)
        {
            movableObject.TurnOffHighlight();
        }

        showHighlighting = false;
        IsActivated = false;
    }
}
