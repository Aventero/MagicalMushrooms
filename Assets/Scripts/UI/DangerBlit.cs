using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DangerBlit : MonoBehaviour
{
    public Material BlitMaterial;
    private ScriptableRendererFeature ScriptableRenderer;
    public Outline WitchOutline;

    [ColorUsageAttribute(true, true)]
    public Color PlayerIsSafe;
    [ColorUsageAttribute(true, true)]
    public Color PlayerInDanger;
    [ColorUsageAttribute(true, true)]
    public Color WitchAttacking;
    [ColorUsageAttribute(true, true)]
    public Color Damage;


    private DangerState dangerState;

    // How long till max is reached
    private const float DangerTime = 5f;
    private float dangerTimer = 0f;
    private float safeTimer = 0.5f;
    private float damageTimer = 0.5f;
    private float outlineFadeTimer = 0f;

    void Start()
    {
        // Kinda hacky way to get the renderFeatures https://forum.unity.com/threads/how-to-get-access-to-renderer-features-at-runtime.1193572/
        // Find the Blit render feature thats assignet by a Quality setting
        var renderer = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).GetRenderer(0);
        var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
        List<ScriptableRendererFeature> features = property.GetValue(renderer) as List<ScriptableRendererFeature>;
        ScriptableRendererFeature blit = features.Find((feature) => feature.name == "Blit");
        ScriptableRenderer = blit;
    }

    public void SetState(DangerState state)
    {
        if (ScriptableRenderer == null)
            return;

        dangerState = state;

        switch (dangerState)
        {
            case DangerState.Safe:
                BlitMaterial.SetColor("_Color", PlayerIsSafe);
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.green;
                break;
            case DangerState.Danger:
                BlitMaterial.SetColor("_Color", PlayerInDanger);
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.yellow;
                break;
            case DangerState.Attack:
                BlitMaterial.SetColor("_Color", WitchAttacking);
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.red;
                break;
            case DangerState.Damage:
                BlitMaterial.SetColor("_Color", Damage);
                WitchOutline.OutlineMode = Outline.Mode.OutlineAll;
                WitchOutline.OutlineColor = Color.magenta;
                break;
            case DangerState.Nothing:
                WitchOutline.OutlineColor = Color.white;
                WitchOutline.OutlineMode = Outline.Mode.OutlineVisible;
                ScriptableRenderer.SetActive(false);
                BlitMaterial.SetFloat("_Transparency", 0);
                break;
        }

        ScriptableRenderer.SetActive(true);
        ResetBlit();
    }   

    public void ResetBlit()
    {
        dangerTimer = 0f;
        outlineFadeTimer = 0f;
        safeTimer = BlitMaterial.GetFloat("_Transparency");
        damageTimer = BlitMaterial.GetFloat("_Transparency");
    }

    public void UpdateBlit()
    {
        switch (dangerState)
        {
            case DangerState.Safe:
                SafeSetting();
                break;
            case DangerState.Danger:
                DangerSetting();
                break;
            case DangerState.Attack:
                AttackSetting();
                break;
            case DangerState.Damage:
                DamageSetting();
                break;
            case DangerState.Nothing:
                NothingSetting();
                ScriptableRenderer.SetActive(false);
                BlitMaterial.SetFloat("_Transparency", 0);
                break;
        }
    }

    private void DangerSetting()
    {
        dangerTimer += Time.deltaTime;
        float t = dangerTimer / DangerTime;
        SetBlitValue(Mathf.Clamp(t, 0, 0.7f)); 
    }

    private void SafeSetting()
    {
        safeTimer -= Time.deltaTime;
        SetBlitValue(Mathf.Clamp(safeTimer, 0, 1));

        // Fade outline to white
        outlineFadeTimer += Time.deltaTime;
        WitchOutline.OutlineColor = Color.Lerp(WitchOutline.OutlineColor, new Color(1, 1, 1, 0), Mathf.Clamp(outlineFadeTimer, 0f, 1f));
    }

    private void AttackSetting()
    {
    }

    private void DamageSetting()
    {
        damageTimer -= Time.deltaTime;
        SetBlitValue(Mathf.Clamp(damageTimer, 0, 1f));
    }

    private void NothingSetting()
    {
    }

    private void SetBlitStateActive(bool state)
    {
        ScriptableRenderer.SetActive(state);
    }

    private void SetBlitValue(float value)
    {
        BlitMaterial.SetFloat("_Transparency", value);
    }

    private void OnDisable()
    {
        ScriptableRenderer.SetActive(false);
        BlitMaterial.SetFloat("_Transparency", 0);
    }

    IEnumerator LerpBlitCoroutine(float toLerpTo, float time, bool state)
    {
        // Activate if necessary
        SetBlitStateActive(state);

        // Lerp the Transparency to b
        float a = BlitMaterial.GetFloat("_Transparency");
        float delta = 0f;
        while (delta < time)
        {
            delta += Time.deltaTime;
            float val = Mathf.Lerp(a, toLerpTo, delta / time);
            SetBlitValue(val);
            yield return null;
        }

        // Deactivate if necessary
        SetBlitStateActive(state);
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