using UnityEngine;

public class SmokeBomb: MonoBehaviour
{
    [Header("Throwing")]
    public float mass = 5;
    public float throwStrength = 5f;

    [Header("Line")]
    public int linePoints = 25;
    public float timeBetweenPoints = 0.1f;

    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform releaseTransform;

    private LayerMask ignoredLayer;

    private void Start()
    {
        ignoredLayer |= (1 << LayerMask.GetMask("Player"));
    }

    public void DrawProjection()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
        
        Vector3 startPosition = releaseTransform.position;
        Vector3 startVelocity = throwStrength * Camera.main.transform.forward / mass;

        int lineIndex = 0;
        lineRenderer.SetPosition(lineIndex, startPosition);

        for(float time = 0; time < linePoints; time += timeBetweenPoints)
        {
            lineIndex++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            Vector3 prevPos = lineRenderer.GetPosition(lineIndex - 1); // Get the last point
            Vector3 lineDirection = prevPos - point;

            if (Physics.Raycast(point, lineDirection.normalized, out RaycastHit hit, lineDirection.magnitude, ignoredLayer))
            {
                lineRenderer.SetPosition(lineIndex, hit.point);
                lineRenderer.positionCount = lineIndex + 1;

                Debug.DrawRay(point, lineDirection.normalized, Color.green);
                return;
            }

            lineRenderer.SetPosition(lineIndex, point);
        }
    }

    public void Update()
    {
        DrawProjection();
    }
}
