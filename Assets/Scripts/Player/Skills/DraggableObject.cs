using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [HideInInspector]
    public float HighlightDistance;

    private Outline outline;
    public bool IsFlying = false;

    void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void ShowMarked()
    {
        outline.enabled = true;
        outline.OutlineWidth = 2f;
        outline.OutlineColor = Color.white;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
    }

    public void ShowActivelySelected()
    {
        outline.enabled = true;
        outline.OutlineWidth = 5f;
        outline.OutlineColor = Color.magenta;
        outline.OutlineMode = Outline.Mode.OutlineAll;
    }

    public void HideMarked()
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
            aIStateManager.TransitionToState(AIStates.Stun);
            IsFlying = false;
            Destroy(gameObject); // destroy myself
        }
    }

    private void OnDestroy()
    {
        DraggableManager.Instance.RemoveDraggableFromList(this);
    }
}
