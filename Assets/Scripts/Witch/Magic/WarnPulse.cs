using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WarnPulse : MonoBehaviour
{
    public Volume volume;
    private Vignette vignette;

    public float pulseSpeed = 1.0f;  // How fast the pulsing effect happens
    public float maxIntensity = 0.5f;  // The maximum intensity of the vignette effect
    public float minIntensity = 0.0f;  // The minimum intensity of the vignette effect

    void Start()
    {
        if (volume.profile.TryGet(out vignette))
        {
            vignette.active = true;
        }
    }

    void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (pulse + 1) / 2);
        vignette.intensity.value = intensity;
    }
}