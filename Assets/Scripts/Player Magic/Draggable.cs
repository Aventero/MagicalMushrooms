using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    public bool ResetPosition = false;

    private void Start()
    {
        // Store the initial transform values when the object is created
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    public void ResetToInitialTransform()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
    }

    private void Update()
    {
        if (ResetPosition)
        {
            ResetToInitialTransform();
            ResetPosition = false;
        }
    }
}
