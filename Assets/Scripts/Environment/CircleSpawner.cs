using System.Collections;
using UnityEngine;

public sealed class CircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public float yPosition = -0.1f;

    // Singleton instance
    private static CircleSpawner _instance;

    // Public static property to access the instance
    public static CircleSpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                // Find an active CircleSpawner instance or create a new one if none exists
                _instance = FindObjectOfType<CircleSpawner>() ?? new GameObject("CircleSpawner").AddComponent<CircleSpawner>();
            }
            return _instance;
        }
    }

    // Ensure that the instance is unique
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Spawn a circle without growing
    public GameObject Spawn(Vector3 position, float circleSize)
    {
        GameObject newCircle = Instantiate(circlePrefab, position, Quaternion.identity);
        newCircle.transform.localScale = new Vector3(circleSize, circleSize, 1);

        return newCircle;
    }

    public void SpawnAndGrow(float lifetime, float growthSpeed, Vector3 position, Vector3 orientation)
    {
        // Create a rotation that looks in the direction of 'orientation' with 'Vector3.up' as the upwards direction.
        Quaternion rotation = Quaternion.LookRotation(orientation, Vector3.up);

        GameObject newCircle = Instantiate(circlePrefab, position, rotation);
        StartCoroutine(Grow(newCircle, lifetime, growthSpeed));
    }

    // Coroutine to grow the circle over time
    private IEnumerator Grow(GameObject circle, float lifetime, float growthSpeed)
    {
        float timer = 0;
        SpriteRenderer circleRenderer = circle.GetComponentInChildren<SpriteRenderer>();
        Color initialColor = circleRenderer.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

        float circleSize = 0;

        while (timer < lifetime)
        {
            circleSize += growthSpeed * Time.deltaTime;
            circle.transform.localScale = new Vector3(circleSize, circleSize, circleSize);
            circleRenderer.color = Color.Lerp(initialColor, targetColor, timer / lifetime);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(circle);
    }

}
