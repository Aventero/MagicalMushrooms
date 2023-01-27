using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public float MaxRayDistance;
    public float BoxRaySize;
    public LayerMask layerMask;

    private Interactable oldNearestInteractable;

    private void Start()
    {
        oldNearestInteractable = null;
    }

    private void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.E) && oldNearestInteractable != null)
            oldNearestInteractable.Interact();
    }

    private void FixedUpdate()
    {
        ShootRaycast();
    }

    private void ShootRaycast()
    {
        Transform cameraTransform = Camera.main.transform;

        RaycastHit[] hits = Physics.BoxCastAll(cameraTransform.position, new Vector3(BoxRaySize, BoxRaySize, BoxRaySize), cameraTransform.forward, Quaternion.identity, MaxRayDistance, layerMask);
        List<(Interactable, float)> interactablesAndDistances = GetInteractablesAndDistances(hits);
        Interactable nearestInteractable = GetNearestInteractable(interactablesAndDistances);
        SetNewInteractable(nearestInteractable);
    }

    private void SetNewInteractable(Interactable newInteractable)
    {
        // Nothing has changed
        if (oldNearestInteractable == newInteractable)
            return;

        if (oldNearestInteractable != null)
        {
            oldNearestInteractable.OutOfPlayerSight();
            oldNearestInteractable.Outline.OutlineWidth = 1;
        }

        if (newInteractable != null)
        {
            // Sees the interactable
            newInteractable.InPlayerSight();
            newInteractable.Outline.OutlineWidth = 10;
        }

        oldNearestInteractable = newInteractable;
    }

    private Interactable GetNearestInteractable(List<(Interactable, float)> interactables)
    {
        if (interactables.Count <= 0)
            return null;

        (Interactable, float) nearestInteractable = interactables[0];

        foreach ((Interactable, float) interactable in interactables)
        {
            if (nearestInteractable.Item2 < interactable.Item2)
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
            interactables.Add((interactable, hit.distance));
        }

        return interactables;
    }

    private void OnDrawGizmos()
    {
        Transform cameraTransform = Camera.main.transform;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * BoxRaySize);

        Gizmos.DrawWireCube(cameraTransform.position + MaxRayDistance * cameraTransform.forward, new Vector3(2 * BoxRaySize, 2 * BoxRaySize, 2 * BoxRaySize));
    }
}