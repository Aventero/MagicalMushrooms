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
        dangerState = state;
        ScriptableRenderer.SetActive(true);
        ResetBlit();
    }   

    public void ResetBlit()
    {
        dangerTimer = 0f;
        safeTimer = 0.5f;
        damageTimer = 0.3f;
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
                ScriptableRenderer.SetActive(false);
                BlitMaterial.SetFloat("_Transparency", 0);
                break;
        }
    }

    private void DangerSetting()
    {
        BlitMaterial.SetColor("_Color", PlayerInDanger);

        dangerTimer += Time.deltaTime;
        float t = dangerTimer / DangerTime;
        SetBlitValue(Mathf.Clamp(t, 0, 0.7f)); 
    }

    private void SafeSetting()
    {
        BlitMaterial.SetColor("_Color", PlayerIsSafe);
        safeTimer -= Time.deltaTime;
        SetBlitValue(Mathf.Clamp(safeTimer, 0, 1));
    }

    private void AttackSetting()
    {
        BlitMaterial.SetColor("_Color", WitchAttacking);
    }

    private void DamageSetting()
    {
        BlitMaterial.SetColor("_Color", Damage);
        damageTimer -= Time.deltaTime;
        SetBlitValue(Mathf.Clamp(damageTimer, 0, 1f));
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