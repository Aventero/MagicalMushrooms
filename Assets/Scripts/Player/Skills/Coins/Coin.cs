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
    private float currentJiggleAmount = 0f;
    public float MaxJiggleAmount = 1f;
    public float maxJiggleDuration = 0.5f;
    private float currentJiggleDuration = 0f;
    private float jiggleSpeed = 5f; 
    private float timeToChangeDirection = 0.05f; 
    private float directionChangeTimer = 0f;
    private TrailRenderer trailRenderer;

    private void Awake()
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
            float xJiggle = initialPosition.x + Random.Range(-currentJiggleAmount, currentJiggleAmount);
            float yJiggle = initialPosition.y + Random.Range(-currentJiggleAmount, currentJiggleAmount);
            float zJiggle = initialPosition.z + Random.Range(-currentJiggleAmount, currentJiggleAmount);
            targetJigglePosition = new Vector3(xJiggle, yJiggle, zJiggle);
        }

        // Lerp to the target jiggle position
        transform.position = Vector3.Lerp(transform.position, targetJigglePosition, Time.deltaTime * jiggleSpeed);
        currentJiggleAmount = Mathf.Lerp(0, MaxJiggleAmount, currentJiggleDuration / maxJiggleDuration);

        if (currentJiggleDuration >= maxJiggleDuration)
        {
            Vector3 jiggleDirection = (transform.position - initialPosition).normalized;
            CanBeSuckedIn = true; 
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

        float timeSinceLastCloseApproach = 0f;
        float previousDistance = distanceToOrigin;

        while (distanceToOrigin > 0.015f)
        {
            float currentScaleFactor = transform.localScale.x / initialScale.x;
            trailRenderer.startWidth = initialStartWidth * currentScaleFactor;
            trailRenderer.endWidth = initialEndWidth * currentScaleFactor;

            // Scaling down 1 -> 0
            float distanceFactor = 1f - (distanceToOrigin / startDistance);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, distanceFactor);

            // Attraction logic using acceleration and velocity
            velocity += (origin.position - transform.position).normalized * vacuumForce;
            float dampingFactor = 0.05f + 0.9f * (1f - distanceFactor);
            velocity *= dampingFactor;
            velocity = Vector3Extensions.ClampMagnitude(velocity, -100, 100);

            // Check if the coin is getting closer to the player
            if (distanceToOrigin < previousDistance)
                timeSinceLastCloseApproach = 0f;
            else
                timeSinceLastCloseApproach += Time.deltaTime;

            previousDistance = distanceToOrigin;

            // If the coin hasn't made significant progress in 1 second, apply a stronger force
            if (timeSinceLastCloseApproach > 0.1f)
            {
                velocity *= 0.25f; 
                timeSinceLastCloseApproach = 0f;
            }

            // Apply
            transform.position += velocity * Time.deltaTime;

            distanceToOrigin = Vector3.Distance(transform.position, origin.position);
            yield return null;
        }

        Stats.Instance.IncreaseCoinsCollected(CoinData.Value);
        Destroy(gameObject);
    }


    public void ChargeRoutine(Transform origin, float force, Vector3 startDirection, Vector3 startScale)
    {
        StartCoroutine(CounterSlurp(origin, force, startDirection, startScale));
    }

    public IEnumerator CounterSlurp(Transform origin, float force, Vector3 startDirection, Vector3 startScale)
    {
        GetComponent<TrailRenderer>().enabled = true;
        GetComponent<Collider>().enabled = false;
        trailRenderer.time = 0.15f;

        // Variables for acceleration and velocity
        Vector3 velocity = 10f * force * startDirection;

        // Initial scales
        Vector3 initialScale = startScale;
        float initialStartWidth = initialScale.x;
        float initialEndWidth = 0f;

        float startDistance = Vector3.Distance(transform.position, origin.position);
        float distanceToOrigin = startDistance;

        float timeSinceLastCloseApproach = 0f;
        float previousDistance = distanceToOrigin;

        while (distanceToOrigin > 0.1f)
        {
            float currentScaleFactor = transform.localScale.x / initialScale.x;
            trailRenderer.startWidth = initialStartWidth * currentScaleFactor;
            trailRenderer.endWidth = initialEndWidth * currentScaleFactor;

            // Scaling down 1 -> 0
            float distanceFactor = 1f - (distanceToOrigin / startDistance);
            transform.localScale = Vector3.Lerp(initialScale, new Vector3(0.1f, 0.1f, 0.1f), distanceFactor);

            // Attraction logic using acceleration and velocity
            velocity += (origin.position - transform.position).normalized * force;
            float dampingFactor = 0.01f + 0.95f * (1f - distanceFactor);
            //velocity *= dampingFactor;
            velocity = Vector3Extensions.ClampMagnitude(velocity, -100, 100);

            // Check if the coin is getting closer to the player
            if (distanceToOrigin < previousDistance)
                timeSinceLastCloseApproach = 0f;
            else
                timeSinceLastCloseApproach += Time.deltaTime;

            previousDistance = distanceToOrigin;

            // If the coin hasn't made significant progress in 1 second, apply a stronger force
            if (timeSinceLastCloseApproach > 0.01f)
            {
                velocity *= 0.25f;
                timeSinceLastCloseApproach = 0f;
            }

            // Apply
            transform.position += velocity * Time.deltaTime;

            distanceToOrigin = Vector3.Distance(transform.position, origin.position);
            yield return null;
        }

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