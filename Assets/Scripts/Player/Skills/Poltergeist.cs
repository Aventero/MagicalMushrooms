using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Poltergeist : PlayerSkill
{
    [Header("Values")]
    public float PushForce = 10;
    public float HighlightDistance;

    [Header("References")]
    public Material HighlightMaterial;
    public Material FocusMaterial;

    [Header("Particles")]
    public ParticleSystem poltergeistParticleSystem;



    private void Update()
    {
        // Activly looking at an object
        if (IsActivated && DraggableManager.Instance.SelectedObject != null)
        {
            UIManager.Instance.ShowSkillTooltip("Alert!", MouseSide.LeftClick);
        }
    }

    public override bool Execute()
    {
        if (DraggableManager.Instance.SelectedObject == null)
            return false;

        if (Stats.Instance.CoinsCollected <= 0)
            return false;

        if (!DraggableManager.Instance.SelectedObject.TryGetComponent<Rigidbody>(out var rb))
            rb = DraggableManager.Instance.SelectedObject.AddComponent<Rigidbody>();

        // Using
        rb.AddForce(Camera.main.transform.forward * PushForce, ForceMode.Impulse);
        Stats.Instance.DecreaseCoinsCollected(SkillCost);
        AIStateManager aIStateManager = FindObjectOfType<AIStateManager>();;

        if (aIStateManager == null)
            Debug.LogError("Could not find AIStateManager");
        aIStateManager.PointOfInterest = DraggableManager.Instance.SelectedObject.transform.position;
        aIStateManager.TransitionToState(AIStates.Alert);
        HidePreview();
        return true;
    }

    public override void ShowPreview()
    {
        DraggableManager.Instance.EnableSearch();
        poltergeistParticleSystem.Play();
        IsActivated = true;
    }

    public override void HidePreview()
    {
        DraggableManager.Instance.DisableSearch();
        poltergeistParticleSystem.Stop();
        UIManager.Instance.HideTooltip();
        IsActivated = false;
    }
}
