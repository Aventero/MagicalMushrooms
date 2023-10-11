using System.Collections;
using UnityEngine;

public class Flipping : MonoBehaviour
{
    public AnimationCurve heightCurve;
    public float Height = 5f;
    public float FlipWaitTime = 5f;
    public float FlipTime = 2.0f;
    public float RotationSpeed = 100f;
    private Vector3 center;
    private Vector3 firstPosition;

    // Start is called before the first frame update
    void Start()
    {
        firstPosition = transform.position;
        center = GetComponent<MeshFilter>().mesh.bounds.center;
        center = transform.TransformPoint(center);
        StartCoroutine(BetterFlip(firstPosition));
    }

    IEnumerator BetterFlip(Vector3 initialPosition)
    {
        yield return new WaitForSeconds(FlipWaitTime + Random.value * 10);

        float targetAngle = 180f;
        float currentAngle = 0f;
        float percentComplete = 0;
        Collider collider = GetComponent<Collider>();
        while (percentComplete < 1)
        {
            float angleToRotate = RotationSpeed * Time.deltaTime;
            if (percentComplete <= 0.5)
                transform.position = new Vector3(transform.position.x, initialPosition.y + (heightCurve.Evaluate(percentComplete) * Height), transform.position.z);
            else
            {
                transform.position = new Vector3(transform.position.x, initialPosition.y + (heightCurve.Evaluate(percentComplete) * Height) + (collider.bounds.size.y * Mathf.Lerp(-1, 1, percentComplete)), transform.position.z);
            }
            transform.RotateAround(collider.bounds.center, Vector3.right, angleToRotate);
            currentAngle += angleToRotate;
            percentComplete = currentAngle / targetAngle;
            yield return null;
        }

        yield return new WaitForSeconds(FlipWaitTime);

        initialPosition = transform.position;
        targetAngle = 180f;
        currentAngle = 0f;
        percentComplete = 0;
        while (percentComplete < 1)
        {
            float angleToRotate = RotationSpeed * Time.deltaTime;
            if (percentComplete <= 0.5)
                transform.position = new Vector3(transform.position.x, initialPosition.y + (heightCurve.Evaluate(percentComplete) * Height), transform.position.z);
            else
            {
                transform.position = new Vector3(transform.position.x, initialPosition.y + (heightCurve.Evaluate(percentComplete) * Height) - (collider.bounds.size.y * Mathf.Lerp(-1, 1, percentComplete)), transform.position.z);
            }
            transform.RotateAround(collider.bounds.center, Vector3.right, angleToRotate);
            currentAngle += angleToRotate;
            percentComplete = currentAngle / targetAngle;
            yield return null;
        }

        StartCoroutine(BetterFlip(firstPosition));
    }
}
