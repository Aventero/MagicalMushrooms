using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public float yPosition = -0.1f;

    public void SpawnAndGrowCircle(float lifetime, float maxCircleSize)
    {
        GameObject newCircle = Instantiate(circlePrefab, transform.position + new Vector3(0, yPosition, 0), circlePrefab.transform.rotation);
        StartCoroutine(GrowCircle(newCircle, lifetime, maxCircleSize));
    }

    private IEnumerator GrowCircle(GameObject circle, float lifetime, float maxCircleSize)
    {
        float timer = 0;
        SpriteRenderer circleRenderer = circle.GetComponent<SpriteRenderer>();

        Color initialColor = circleRenderer.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0); // Fully transparent

        while (timer < lifetime)
        {
            float normalizedTime = timer / lifetime;
            float circleSize = Mathf.Lerp(0, maxCircleSize, normalizedTime);

            circle.transform.localScale = new Vector3(circleSize, circleSize, 1);
            circleRenderer.color = Color.Lerp(initialColor, targetColor, normalizedTime);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(circle); // Optional: Remove the circle once it's done fading
    }
   
}