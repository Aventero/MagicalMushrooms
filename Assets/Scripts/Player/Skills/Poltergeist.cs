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
        if (IsActivated && DraggableManager.Instance.DraggableObject != null)
        {
            UIManager.Instance.ShowSkillTooltip("Alert!", MouseSide.LeftClick);
        }
    }

    public override bool Execute()
    {
        if (DraggableManager.Instance.DraggableObject == null)
            return false;

        if (Stats.Instance.CoinsCollected <= 0)
            return false;

        if (!DraggableManager.Instance.DraggableObject.TryGetComponent<Rigidbody>(out var rb))
            rb = DraggableManager.Instance.DraggableObject.AddComponent<Rigidbody>();

        // Using
        rb.AddForce(Camera.main.transform.forward * PushForce, ForceMode.Impulse);
        Stats.Instance.DecreaseCoinsCollected(SkillCost);

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
