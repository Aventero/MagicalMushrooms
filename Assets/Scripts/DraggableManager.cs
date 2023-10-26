using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableManager : MonoBehaviour
{
    public static DraggableManager Instance { get; private set; }

    [Header("Outline")]
    public float outlineWidth = 2f;
    public Color outlineColor = Color.white;
    public DraggableObject DraggableObject { get; private set; }

    [SerializeField] private float rayDistance = 15f;
    private bool shouldSearchDraggables = false;

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
    }

    private void FixedUpdate()
    {
        if (shouldSearchDraggables)
            SelectDraggable();
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

    private void InstantiateDraggables()
    {
        foreach (GameObject draggableGo in GameObject.FindGameObjectsWithTag("Draggable"))
        {
            // Add Draggable 
            if (draggableGo.GetComponent<DraggableObject>() == null)
                draggableGo.AddComponent<DraggableObject>();

            // Add Outline
            Outline outline = draggableGo.GetComponent<Outline>();
            if (outline == null)
                outline = draggableGo.AddComponent<Outline>();

            outline.enabled = false;
            outline.OutlineWidth = outlineWidth;
            outline.OutlineColor = outlineColor;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    public void SelectDraggable()
    {
        DraggableObject target = FindClosestTargetWithScript<DraggableObject>();

        if (target != null)
        {
            if (DraggableObject == null)
            {
                SetDraggable(target);
            }
            else if (DraggableObject != target)
            {
                LoseDraggable();
                SetDraggable(target);
            }
        }
        else if (DraggableObject != null)
        {
            LoseDraggable();
        }
    }

    private void SetDraggable(DraggableObject draggable)
    {
        if (draggable != null)
        {
            DraggableObject = draggable;
            draggable.ShowSelected();
        }
    }

    private void LoseDraggable()
    {
        if (DraggableObject != null)
        {
            DraggableObject.HideSelected();
            DraggableObject = null;
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
