using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    public float MaxLevitationTime = 3f;
    public float UpwardsLevitationAmount = 1.5f;
    public float SidewayLevitationAmount = 1.5f;
    public float Speed = 30f;
    public float Radius = 2f;

    public bool ResetPosition = false;
    public AnimationCurve levitationCurveX;
    public AnimationCurve levitationCurveY;
    public AnimationCurve levitationCurveZ;

    private void Start()
    {
        // Store the initial transform values when the object is created
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    public void ResetToInitial()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        GetComponent<Rigidbody>().isKinematic = true;
        Levitate();
    }

    public void Levitate()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        StartCoroutine(LevitateWithCurve());
    }

    IEnumerator LevitateWithCurve()
    {
        float deltaTime = 0;

        Vector3 center = transform.position;
        float angle = 0f;
        GetComponent<BoxCollider>().enabled = false;
        while(deltaTime < MaxLevitationTime)
        {
            float x = center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;
            float z = center.z + Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
            transform.position = new Vector3(x, initialPosition.y + levitationCurveY.Evaluate(deltaTime / MaxLevitationTime), z);
            angle += Speed * Time.deltaTime;
            if (angle >= 360)
                angle = 0;

            deltaTime += Time.deltaTime;
            yield return null;
        }
        GetComponent<BoxCollider>().enabled = false;
        yield return null;
    }

    private void Update()
    {
        if (ResetPosition)
        {
            ResetToInitial();
            ResetPosition = false;
        }
    }

}
