using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassSlurpSpin : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 startingPosition;
    private Quaternion initialRotation;

    public float upwardMovementDuration = 2.0f;
    public float upwardMovementDistance = 1.0f;
    public float rotationSpeed = 30f;
    private bool isAnimating = false;
    private float timeAnimating = 0.0f; 
    private float currentRotation = 0.0f;

    private ParticleSystem glowingHalo;
    public ParticleSystem SlurpRadius;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        glowingHalo = transform.Find("GlowingHalo").GetComponent<ParticleSystem>();
        glowingHalo.Stop();
        SlurpRadius.Stop();
    }

    private void Update()
    {
        if (isAnimating)
        {
            timeAnimating += Time.deltaTime;

            // Spin!
            currentRotation += rotationSpeed * Time.deltaTime;
            transform.localRotation = initialRotation * Quaternion.Euler(0, currentRotation, 0);

            // Move upward!
            if (timeAnimating <= upwardMovementDuration)
            {
                float upwardProgress = timeAnimating / upwardMovementDuration;
                transform.localPosition = Vector3.Lerp(startingPosition, initialPosition + Vector3.up * upwardMovementDistance, upwardProgress);
            }
            else
            {
                transform.localPosition = initialPosition + Vector3.up * upwardMovementDistance;
            }
        }
    }

    public void StartAnimating()
    {
        isAnimating = true;
        startingPosition = transform.localPosition; // Store the position when the animation starts

        // Current relative Y-rotation
        currentRotation = Quaternion.Angle(initialRotation, transform.localRotation);

        timeAnimating = 0.0f;
        StartGlowingHalo();
    }
    public void StopAnimating()
    {
        isAnimating = false;
        StartCoroutine(ReturnToInitialPosition());
        StopGlowingHalo();
    }

    private void StartGlowingHalo()
    {
        if (glowingHalo)
        {
            glowingHalo.Play();
            SlurpRadius.Play();
        }
    }

    private void StopGlowingHalo()
    {
        if (glowingHalo)
        {
            glowingHalo.Stop();
            SlurpRadius.Stop();
        }
    }

    private IEnumerator ReturnToInitialPosition()
    {
        while (Vector3.Distance(transform.localPosition, initialPosition) > 0.01f || Quaternion.Angle(transform.localRotation, initialRotation) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 2);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.deltaTime * 2);
            yield return null;
        }
        // Snap to the exact initial position and rotation once close enough
        transform.SetLocalPositionAndRotation(initialPosition, initialRotation);
    }
}
