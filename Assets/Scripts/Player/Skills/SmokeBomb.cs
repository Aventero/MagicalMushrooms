using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SmokeBomb : PlayerSkill
{
    [Header("Throwing")]
    public float mass = 3;
    public float throwStrength = 15;
    public Vector3 cameraAngleAdjustment = new(0, 0.8f, 0);

    [Header("Trajectory Line")]
    public int linePoints = 25;
    public float timeBetweenPoints = 0.1f;
    public float circleSize;

    [Header("Smoke Bomb")]
    public float growthTime = 5; // In seconds
    public Vector3 maxGrowingScale;

    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform releaseTransform;
    public GameObject circle;
    public GameObject smokeEffect;
    public GameObject throwingObject;

    private LayerMask allowedLayers;
    private GameObject smokeCircle;
    private CircleSpawner circleSpawner;

    private bool drawProjection = false;
    private Vector3 lastHit;

    private void Start()
    {
        allowedLayers = LayerMask.GetMask("Default", "Prop");
        circleSpawner = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleSpawner>();
    }

    public override void ShowPreview()
    {
        UIManager.Instance.ShowTooltip(TooltipText, MouseSide.LeftClick);

        lineRenderer.enabled = true;
        drawProjection = true;

        if (IsActivated)
            HidePreview();
        else
            IsActivated = true;
    }

    public override void HidePreview()
    {
        UIManager.Instance.HideTooltip();

        IsActivated = false;
        drawProjection = false;

        lineRenderer.enabled = false;
        Destroy(smokeCircle);
    }

    public override bool Execute()
    {
        IsActivated = false;

        GameObject throwGameObject = Instantiate(throwingObject);
        throwGameObject.transform.position = releaseTransform.position;

        Rigidbody throwRigidbody = throwGameObject.GetComponent<Rigidbody>();
        throwRigidbody.mass = mass;
        throwRigidbody.AddForce((Camera.main.transform.forward + cameraAngleAdjustment) * throwStrength, ForceMode.Impulse);

        HidePreview();

        return true;
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

            if (Physics.Raycast(prevPos, lineDirection.normalized, out RaycastHit hit, lineDirection.magnitude, allowedLayers))
            {
                Destroy(smokeCircle);
                Debug.DrawRay(point, lineDirection, Color.green);

                lineRenderer.SetPosition(lineIndex, hit.point);
                lineRenderer.positionCount = lineIndex + 1;

                lastHit = hit.point + new Vector3(0, 0.01f, 0);
                smokeCircle = circleSpawner.Spawn(lastHit, circleSize);

                return;
            }

            lineRenderer.SetPosition(lineIndex, point);
        }
    }

    public void LateUpdate()
    {
        if (drawProjection)
            DrawProjection();
    }
}
