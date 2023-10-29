using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SmokeFrame : MonoBehaviour
{
    private float duration = 0.5f;

    public void Reveal()
    {
        StartCoroutine(ChangeTransparency(GetComponent<Image>(), 0f, 0.8f, duration, true));
    }

    public void Hide()
    {
        StartCoroutine(ChangeTransparency(GetComponent<Image>(), 0.8f, 0f, duration, false));
    }

    IEnumerator ChangeTransparency(Image smokeImage, float startAlpha, float endAlpha, float time, bool enable)
    {
        if (smokeImage == null)
        {
            Debug.LogError("No Smoke Frame image found!");
            yield break;
        }

        if (enable)
            smokeImage.enabled = true;

        float elapsedTime = 0f;
        Color currentColor = smokeImage.color;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / time);
            smokeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        smokeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, endAlpha);

        if (!enable)
            smokeImage.enabled = false;
    }
}
