using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [HideInInspector]
    public Material HighlightMaterial;
    [HideInInspector]
    public Material FocusMaterial;
    [HideInInspector]
    public float HighlightDistance;

    private Outline outline;
    public bool IsFlying = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void ShowSelected()
    {
        outline.enabled = true;
    }

    public void HideSelected()
    {
        outline.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsFlying)
            return;

        if (other.gameObject.CompareTag("Hitbox"))
        {
            AIStateManager aIStateManager = other.GetComponentInParent<AIStateManager>();
            aIStateManager.TransitionToState(AIStates.LostPlayer);
            IsFlying = false;
        }
    }
}
