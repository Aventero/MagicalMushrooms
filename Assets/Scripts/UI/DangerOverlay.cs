using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DangerOverlay : MonoBehaviour
{
    public Material DangerMaterial;
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
    public float Intensity = 20f;
    private float intensity = 0;
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
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.green;
                break;
            case DangerState.Danger:
                TargetColor = PlayerInDanger;
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.yellow;
                break;
            case DangerState.Attack:
                TargetColor = WitchAttacking;
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.red;
                break;
            case DangerState.Damage:
                TargetColor = Damage;
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.magenta;
                break;
            case DangerState.Nothing:
                TargetColor = Nothing;
                WitchOutline.OutlineColor = Color.white;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                break;
        }
        OldColor = DangerMaterial.color;
        fadeTimer = 0;
    }   


    public void UpdateBlit()
    {
        fadeTimer += Time.deltaTime;
        lerpPercent = Mathf.Clamp(fadeTimer / transitionDuration, 0, 1);
        FadeColor();


        if ( lerpPercent < 0.5)
        {
            intensity = Mathf.Lerp(Intensity, 0, lerpPercent * 2);
        } 
        else
        {
            intensity = Mathf.Lerp(0, intensity, lerpPercent);
        }
    }

    private void FadeIntensity()
    {

    }

    private void FadeColor()
    {
        DangerMaterial.color = Color.Lerp(OldColor, TargetColor * Intensity, lerpPercent);
    }

    private void OnDisable()
    {
        DangerMaterial.color = Nothing;
    }

    private void OnDestroy()
    {
        DangerMaterial.color = Nothing;
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