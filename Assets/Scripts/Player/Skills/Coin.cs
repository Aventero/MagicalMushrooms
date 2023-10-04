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
            CanBeSuckedIn = true; // Enable the flag when max jiggle duration is reached
            StartCoroutine(TheSlurp(origin, slurpForce));
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

    public IEnumerator TheSlurp(Transform origin, float vacuumForce)
    {
        trailRenderer.enabled = true;
        // Management
        GetComponent<Collider>().enabled = false;
        IsSlurping = true;

        // Scale
        Vector3 initialScale = transform.localScale;

        // Trail renderer
        float initialStartWidth = trailRenderer.startWidth;
        float initialEndWidth = trailRenderer.endWidth;

        // Lerping
        float startDistance = Vector3.Distance(transform.position, origin.position);
        float distanceToOrigin = startDistance;
        while (distanceToOrigin > 0.001f)
        {
            // Adjust Trailrenderer
            float currentScaleFactor = transform.localScale.x / initialScale.x;
            trailRenderer.startWidth = initialStartWidth * currentScaleFactor;
            trailRenderer.endWidth = initialEndWidth * currentScaleFactor;

            // This will go from 0 to 1 as the coin gets closer
            float distanceFactor = 1f - (distanceToOrigin / startDistance);

            // Force increase by distance
            float currentForce = vacuumForce * (1f + Mathf.Pow(distanceFactor, ExponentialIncrease));

            // Scale down by distance
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, distanceFactor);

            // Coin moves to origin
            float adjustedForce = currentForce * (1f - CoinData.Resistance);
            transform.position = Vector3.MoveTowards(
                transform.position,
                origin.position,
                adjustedForce * Time.deltaTime
            );

            distanceToOrigin = Vector3.Distance(transform.position, origin.position);
            yield return null;
        }

        Destroy(gameObject);
    }
}
