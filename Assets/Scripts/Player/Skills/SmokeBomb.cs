using UnityEngine;

public class SmokeBomb: MonoBehaviour
{
    [Header("Throwing")]
    public float mass = 3;
    public float throwStrength = 15;
    public Vector3 cameraAngleAdjustment = new(0, 0.8f, 0);

    [Header("Line")]
    public int linePoints = 25;
    public float timeBetweenPoints = 0.1f;
    public float circleSize;

    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform releaseTransform;
    public GameObject circle;

    private LayerMask ignoredLayer;
    private GameObject smokeCircle;
    private CircleSpawner circleSpawner;

    private bool drawProjection = false;
    public bool IsActivated { get; private set; }

    private void Start()
    {
        ignoredLayer |= (1 << LayerMask.GetMask("Player"));
        circleSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleSpawner>();
    }

    public void Activate()
    {
        lineRenderer.enabled = true;
        drawProjection = true;

        if (IsActivated)
            Deactivate();
        else
            IsActivated = true;
    }

    public void Deactivate()
    {
        IsActivated = false;
        drawProjection = false;

        lineRenderer.enabled = false;
        Destroy(smokeCircle);
    }

    public void DrawProjection()
    {
        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
        
        Vector3 startPosition = releaseTransform.position;
        Vector3 startVelocity = throwStrength * (Camera.main.transform.forward + cameraAngleAdjustment) / mass;

        int lineIndex = 0;
        lineRenderer.SetPosition(lineIndex, startPosition);

        for(float time = 0; time < linePoints; time += timeBetweenPoints)
        {
            lineIndex++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            Vector3 prevPos = lineRenderer.GetPosition(lineIndex - 1); // Get the last point
            Vector3 lineDirection = point - prevPos;

            if (Physics.Raycast(point, lineDirection.normalized, out RaycastHit hit, lineDirection.magnitude, ignoredLayer))
            {
                Destroy(smokeCircle);
                Debug.DrawRay(point, lineDirection, Color.green);

                lineRenderer.SetPosition(lineIndex, hit.point);
                lineRenderer.positionCount = lineIndex + 1;

                smokeCircle = circleSpawner.Spawn(hit.point + new Vector3(0, 0.01f, 0), circleSize);

                return;
            }

            lineRenderer.SetPosition(lineIndex, point);
        }
    }

    public void Update()
    {
        if (drawProjection)
            DrawProjection();
    }
}
