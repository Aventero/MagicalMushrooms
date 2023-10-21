using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DangerOverlay : MonoBehaviour
{
    public Material UIDangerMaterial;
    public Material NormalDangerMaterial;
    public Outline WitchOutline;

    [ColorUsageAttribute(true, true)]
    public Color Nothing;
    [ColorUsageAttribute(true, true)]
    public Color PlayerIsSafe;
    [ColorUsageAttribute(true, true)]
    public Color PlayerInDanger;
    [ColorUsageAttribute(true, true)]
    public Color WitchAttacking;
    [ColorUsageAttribute(true, true)]
    public Color Damage;
    public float UIIntensity = 0f;
    public float NormalIntensity = 0f;
    [ColorUsageAttribute(true, true)]
    public Color TargetColor;
    [ColorUsageAttribute(true, true)]
    public Color OldColor;

    private DangerState dangerState;

    // How long till max is reached
    float transitionDuration = 1.0f; // Adjust to your needs
    float fadeTimer = 0f;
    float lerpPercent = 0;


    public void SetState(DangerState state)
    {
        dangerState = state;
        switch (dangerState)
        {
            case DangerState.Safe:
                TargetColor = PlayerIsSafe;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                WitchOutline.OutlineColor = Color.green;
                break;
            case DangerState.Danger:
                TargetColor = PlayerInDanger;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                WitchOutline.OutlineColor = Color.yellow;
                break;
            case DangerState.Attack:
                TargetColor = WitchAttacking;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                WitchOutline.OutlineColor = Color.red;
                break;
            case DangerState.Damage:
                TargetColor = Damage;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                WitchOutline.OutlineColor = Color.magenta;
                break;
            case DangerState.Nothing:
                TargetColor = Nothing;
                WitchOutline.OutlineColor = Color.white;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                break;
        }
        OldColor = UIDangerMaterial.color;
        fadeTimer = 0;
    }   


    public void UpdateColors()
    {
        fadeTimer += Time.deltaTime;
        lerpPercent = Mathf.Clamp(fadeTimer / transitionDuration, 0, 1);
        FadeColor();
    }

    private void FadeColor()
    {
        UIDangerMaterial.color = Color.Lerp(OldColor, TargetColor * UIIntensity, lerpPercent);
        NormalDangerMaterial.color = Color.Lerp(OldColor, TargetColor * NormalIntensity, lerpPercent);
    }

    private void OnDisable()
    {
        UIDangerMaterial.color = Nothing;
    }

    private void OnDestroy()
    {
        UIDangerMaterial.color = Nothing;
    }
}
public enum DangerState
{
    Safe,
    Danger,
    Attack,
    Damage,
    Nothing
}