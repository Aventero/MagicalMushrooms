using System.Collections;
using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public float yPosition = -0.1f;

    public GameObject Spawn(Vector3 position, float circleSize)
    {
        GameObject newCircle = Instantiate(circlePrefab, position, circlePrefab.transform.rotation);
        SpriteRenderer spriteRenderer = newCircle.GetComponent<SpriteRenderer>();
        spriteRenderer.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(circleSize, circleSize, 1);

        return newCircle;
    }

    public void SpawnAndGrow(float lifetime, float maxCircleSize)
    {
        GameObject newCircle = Instantiate(circlePrefab, transform.position + new Vector3(0, yPosition, 0), circlePrefab.transform.rotation);
        StartCoroutine(Grow(newCircle, lifetime, maxCircleSize));
    }

    private IEnumerator Grow(GameObject circle, float lifetime, float maxCircleSize)
    {
        float timer = 0;
        SpriteRenderer circleRenderer = circle.GetComponent<SpriteRenderer>();

        Color initialColor = circleRenderer.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        while (timer < lifetime)
        {
            float normalizedTime = timer / lifetime;
            float circleSize = Mathf.Lerp(0, maxCircleSize, normalizedTime);

            circle.transform.localScale = new Vector3(circleSize, circleSize, 1);
            circleRenderer.color = Color.Lerp(initialColor, targetColor, normalizedTime);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(circle);
    }
   
}