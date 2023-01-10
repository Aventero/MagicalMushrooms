using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public float MaxRayDistance;

    private List<Interactable> OnSightInteractablesList;

    private void Start()
    {
        OnSightInteractablesList = new List<Interactable>();
    }

    // Looking for Interactable in Range and Sight
    private void FixedUpdate()
    {
        Transform cameraTransform = Camera.main.transform;

        RaycastHit[] hits = Physics.BoxCastAll(cameraTransform.position, new Vector3(0.5f, 0.5f, 0.5f), cameraTransform.forward, Quaternion.identity, MaxRayDistance);
        UpdateInteractablesList(hits);

    }

    private void UpdateInteractablesList(RaycastHit[] hits)
    {
        List<Interactable> newInteractables = GetInteractables(hits);

        CleanUpOnSightInteractablesList(newInteractables);
        AddNewInteractables(newInteractables);
    }

    private void CleanUpOnSightInteractablesList(List<Interactable> newInteractables)
    {
        List<Interactable> removeInteractablesList = new List<Interactable>();
        foreach (Interactable oldinteractables in OnSightInteractablesList)
        {
            if (newInteractables.Contains(oldinteractables))
                continue;

            oldinteractables.OutOfPlayerSight();
            removeInteractablesList.Add(oldinteractables);
            
        }

        foreach (Interactable removeInteractable in removeInteractablesList)
            OnSightInteractablesList.Remove(removeInteractable);
    }

    private void AddNewInteractables(List<Interactable> newInteractables)
    {
        foreach (Interactable newInteractable in newInteractables)
        {
            if (OnSightInteractablesList.Contains(newInteractable))
                continue;

            newInteractable.InPlayerSight();
            OnSightInteractablesList.Add(newInteractable);
        }
    }

    private List<Interactable> GetInteractables(RaycastHit[] hits)
    {
        List<Interactable> interactables = new List<Interactable>();

        foreach (RaycastHit hit in hits)
        {
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            if (interactable != null)
                interactables.Add(interactable);
        }

        return interactables;
    }
}