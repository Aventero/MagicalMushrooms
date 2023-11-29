using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableManager : MonoBehaviour
{
    public static DraggableManager Instance { get; private set; }

    public DraggableObject SelectedObject { get; private set; }

    [SerializeField] private float rayDistance = 15f;
    private bool shouldSearchDraggables = false;
    private Transform player;

    private readonly HashSet<DraggableObject> draggables = new();
    private readonly HashSet<DraggableObject> markedDraggables = new();

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InstantiateDraggables();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (shouldSearchDraggables)
        {
            FindPotentialDraggablesInRange();
            SelectDraggable();
        }
        else
        {
            if (markedDraggables.Count > 0)
                HideSuitableDraggables();
        }
    }

    private void FindPotentialDraggablesInRange()
    {
        List<DraggableObject> toAdd = new List<DraggableObject>();
        List<DraggableObject> toRemove = new List<DraggableObject>();

        float distance = rayDistance;

        foreach (var draggable in draggables)
        {
            if (draggable == SelectedObject)
                continue;

            float distanceToPlayer = (player.position - draggable.transform.position).magnitude;

            if (distanceToPlayer <= distance)
            {
                if (!markedDraggables.Contains(draggable))
                {
                    toAdd.Add(draggable);
                }
                draggable.ShowMarked();
            }
            else
            {
                if (markedDraggables.Contains(draggable))
                {
                    toRemove.Add(draggable);
                }
                draggable.HideMarked();
            }
        }

        // Now, safely modify the markedDraggables collection outside the loop
        foreach (var item in toAdd)
        {
            markedDraggables.Add(item);
        }

        foreach (var item in toRemove)
        {
            markedDraggables.Remove(item);
        }
    }


    private void HideSuitableDraggables()
    {
        foreach (var draggable in markedDraggables)
        {
            draggable.HideMarked();
        }

        markedDraggables.Clear();
    }

    public void EnableSearch()
    {
        shouldSearchDraggables = true;
    }

    public void DisableSearch()
    {
        shouldSearchDraggables = false;
        LoseDraggable();
    }

    public void RemoveDraggableFromList(DraggableObject draggable)
    {
        draggables.Remove(draggable);
    }

    private void InstantiateDraggables()
    {
        foreach (GameObject draggableGo in GameObject.FindGameObjectsWithTag("Draggable"))
        {
            if (draggableGo.GetComponent<MeshRenderer>() == null && draggableGo.GetComponent<Collider>() == null)
                continue;

            // Add Draggable 
            DraggableObject draggableObject = draggableGo.GetComponent<DraggableObject>();
            if (draggableObject == null)
                draggableObject = draggableGo.AddComponent<DraggableObject>();

            // Add Outline
            if (draggableGo.isStatic)
            {
                Debug.LogError("Draggable object is static: " + draggableGo.name + " cannot add outline.");
                continue;
            }

            Outline outline = draggableGo.GetComponent<Outline>();
            if (outline == null)
                outline = draggableGo.AddComponent<Outline>();
            outline.enabled = false;

            draggables.Add(draggableObject);
        }
    }

    public void SelectDraggable()
    {
        DraggableObject target = FindClosestTargetWithScript<DraggableObject>();

        if (target != null)
        {
            if (SelectedObject == null)
            {
                SetDraggable(target);
            }
            else if (SelectedObject != target)
            {
                LoseDraggable();
                SetDraggable(target);
            }
        }
        else if (SelectedObject != null)
        {
            LoseDraggable();
        }
    }

    private void SetDraggable(DraggableObject draggable)
    {
        if (draggable != null)
        {
            SelectedObject = draggable;
            draggable.ShowActivelySelected();
        }
    }

    private void LoseDraggable()
    {
        if (SelectedObject != null)
        {
            SelectedObject.HideMarked();
            SelectedObject = null;
        }

        if (UIManager.Instance.GetActiveToolTipType() == ToolTipType.Skill)
            UIManager.Instance.HideTooltip();
    }

    private T FindClosestTargetWithScript<T>() where T : MonoBehaviour
    {
        // Shoot raycast from center of screen and get the scripts with the T script
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        T closestScript = null;
        float minDistance = float.MaxValue; // Start with the maximum value so any distance will be less than this

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<T>(out var script))
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestScript = script;
                }
            }
        }

        return closestScript;
    }
}
