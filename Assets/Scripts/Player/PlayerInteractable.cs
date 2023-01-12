using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public float MaxRayDistance;
    public float BoxRaySize = 1.0f;

    private Interactable nearestInteractable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearestInteractable != null)
            nearestInteractable.Interact();
    }

    private void FixedUpdate()
    {
        Transform cameraTransform = Camera.main.transform;

        RaycastHit[] hits = Physics.BoxCastAll(cameraTransform.position, new Vector3(BoxRaySize, BoxRaySize, BoxRaySize), cameraTransform.forward, Quaternion.identity, MaxRayDistance);
        List<(Interactable, float)> interactablesAndDistances = GetInteractablesAndDistances(hits);
        Interactable nearestInteractable = GetNearestInteractable(interactablesAndDistances);
        SetNewInteractable(nearestInteractable);
    }

    private void SetNewInteractable(Interactable newInteractable)
    {
        if (nearestInteractable != null)
            nearestInteractable.OutOfPlayerSight();

        if(newInteractable != null)
            newInteractable.InPlayerSight();

        nearestInteractable = newInteractable;
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

    private List<(Interactable, float)> GetInteractablesAndDistances(RaycastHit[] hits)
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

    private void OnDrawGizmos()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPos, Camera.main.transform.forward * BoxRaySize);
    }
}