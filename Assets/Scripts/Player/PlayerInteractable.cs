using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public float MaxRayDistance;

    private Interactable oldNearestInteractable;

    private void FixedUpdate()
    {
        Transform cameraTransform = Camera.main.transform;

        RaycastHit[] hits = Physics.BoxCastAll(cameraTransform.position, new Vector3(0.5f, 0.5f, 0.5f), cameraTransform.forward, Quaternion.identity, MaxRayDistance);

        List<(Interactable, float)> interactablesAndDistance = GetInteractablesAndDistance(hits);
        Interactable newNearestInteractable = GetNearestInteractable(interactablesAndDistance);
        SetNewNearestInteractable(newNearestInteractable);
    }

    private void SetNewNearestInteractable(Interactable newNearestInteractable)
    {
        if (newNearestInteractable == oldNearestInteractable)
            return;

        if (oldNearestInteractable != null)
            oldNearestInteractable.OutOfPlayerSight();

        oldNearestInteractable = newNearestInteractable;

        if(newNearestInteractable != null)
            oldNearestInteractable.InPlayerSight();
    }

    private Interactable GetNearestInteractable(List<(Interactable, float)> interactables)
    {
        if (interactables.Count <= 0)
            return null;

        (Interactable, float) nearestInteractable = interactables[0];

        foreach ((Interactable, float) interactable in interactables)
        {
            if (nearestInteractable.Item2 > interactable.Item2)
                nearestInteractable = interactable;
        }

        return nearestInteractable.Item1;
    }

    private List<(Interactable, float)> GetInteractablesAndDistance(RaycastHit[] hits)
    {
        List<(Interactable, float)> interactables = new List<(Interactable, float)>();

        foreach (RaycastHit hit in hits)
        {
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            if (interactable != null)
                interactables.Add((interactable, hit.distance));
        }

        return interactables;
    }
}