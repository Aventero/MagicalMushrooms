using System.Collections;
using UnityEngine;

class Coin : MonoBehaviour
{
    public CoinData CoinData;
    public bool CanBeSuckedIn = false;
    public bool InInitialPosition = true;
    public bool IsSlurping = false;
    public float ExponentialIncrease = 2f;
    private Vector3 initialPosition;
    private Vector3 targetJigglePosition;
    private float jiggleAmount = 0.2f;
    private float maxJiggleDuration = 1f;
    private float currentJiggleDuration = 0f;
    private float jiggleSpeed = 5f; // Adjust to control the speed of the jiggle movement
    private float timeToChangeDirection = 0.05f; // How often to change jiggle direction
    private float directionChangeTimer = 0f;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        initialPosition = transform.position;
        targetJigglePosition = initialPosition;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;
        trailRenderer.startWidth = transform.localScale.x;
    }

    public void Jiggle(Transform origin, float slurpForce)
    {
        if (CanBeSuckedIn) 
            return;

        InInitialPosition = false;

        // Increment the jiggle duration
        currentJiggleDuration += Time.deltaTime;
        directionChangeTimer += Time.deltaTime;

        if (directionChangeTimer >= timeToChangeDirection)
        {
            // Reset the timer
            directionChangeTimer = 0f;

            // Calculate a new target jiggle position around the initial position
            float xJiggle = initialPosition.x + Random.Range(-jiggleAmount, jiggleAmount);
            float yJiggle = initialPosition.y + Random.Range(-jiggleAmount, jiggleAmount);
            targetJigglePosition = new Vector3(xJiggle, yJiggle, initialPosition.z);
        }

        // Lerp to the target jiggle position
        transform.position = Vector3.Lerp(transform.position, targetJigglePosition, Time.deltaTime * jiggleSpeed);

        if (currentJiggleDuration >= maxJiggleDuration)
        {
            Vector3 jiggleDirection = (transform.position - initialPosition).normalized;
            CanBeSuckedIn = true; // Enable the flag when max jiggle duration is reached
            StartCoroutine(TheSlurp(origin, slurpForce, jiggleDirection));
        }
    }

    public void UnJiggle()
    {
        if (CanBeSuckedIn)
            return;

        // Lerp back to the initial position
        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * jiggleSpeed);

        // When close enough to the initial position, reset jiggle duration
        if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
        {
            currentJiggleDuration = 0f;
            InInitialPosition = true;
        }
    }

    public IEnumerator TheSlurp(Transform origin, float vacuumForce, Vector3 startDirection)
    {
        // Management
        trailRenderer.enabled = true;
        GetComponent<Collider>().enabled = false;
        IsSlurping = true;

        // Variables for acceleration and velocity
        Vector3 velocity = startDirection * vacuumForce * 2f;

        // Initial scales
        Vector3 initialScale = transform.localScale;
        float initialStartWidth = trailRenderer.startWidth;
        float initialEndWidth = trailRenderer.endWidth;

        float startDistance = Vector3.Distance(transform.position, origin.position);
        float distanceToOrigin = startDistance;

        while (distanceToOrigin > 0.02f)
        {
            float currentScaleFactor = transform.localScale.x / initialScale.x;
            trailRenderer.startWidth = initialStartWidth * currentScaleFactor;
            trailRenderer.endWidth = initialEndWidth * currentScaleFactor;

            // Scaling down 1 -> 0
            float distanceFactor = 1f - (distanceToOrigin / startDistance);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, distanceFactor);

            // Attraction logic using acceleration and velocity
            velocity += (origin.position - transform.position).normalized * vacuumForce;
            float dampingFactor = 0.05f + 0.9f * (1f - distanceFactor); // Adjust these values as necessary
            velocity *= dampingFactor;

            // Apply
            transform.position += velocity * Time.deltaTime;

            distanceToOrigin = Vector3.Distance(transform.position, origin.position);
            yield return null;
        }

        Stats.Instance.IncreaseCoinsCollected(CoinData.Value);
        Destroy(gameObject);
    }

}

public static class Vector3Extensions
{
    public static Vector3 ClampMagnitude(this Vector3 vector, float min, float max)
    {
        float magnitude = vector.magnitude;

        if (magnitude > max)
        {
            return Vector3.ClampMagnitude(vector, max);
        }
        else if (magnitude < min && magnitude > 0)
        {
            return vector.normalized * min;
        }

        return vector;
    }
}