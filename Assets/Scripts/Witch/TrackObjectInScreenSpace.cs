using UnityEngine;
using UnityEngine.UI;

public class TrackObjectInScreenSpace : MonoBehaviour
{
    [Header("The object thats tracked relative to the rotating source")]
    public Transform pointToTrack;

    [Header("The object limiting the Icons movement")]
    public RectTransform trackingArea;

    [Header("This can be the Witch vision, or player camera")]
    public Transform rotatingSource;
    private RectTransform rectTransform;
    private float halfWidth;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        halfWidth = trackingArea.rect.width * 0.5f;
    }

    private void Update()
    {
        rectTransform.anchoredPosition = GetTrackingPosFromCameraToTrackedObject();
        //rectTransform.anchoredPosition = GetXPosFromSourceForwardToTrackForward();
    }

    private Vector2 GetTrackingPosFromCameraToTrackedObject()
    {
        // camera towards the witch
        Vector3 toEnemy = pointToTrack.position - rotatingSource.position;

        // Project onto the camera's horizontal plane
        Vector3 projectedToTrackPoint = Vector3.ProjectOnPlane(toEnemy, rotatingSource.up);

        // Angle from -180 to 180
        float angle = Vector3.SignedAngle(rotatingSource.forward, projectedToTrackPoint, rotatingSource.up);

        // Clamp the angle between -45 and 45
        float clampedAngle = Mathf.Clamp(angle, -45f, 45f);

        // Map the clamped angle: -45° becomes 0, 45° becomes 1, and 0° becomes 0.5
        float mappedAngle = 1- (0.5f - (clampedAngle / 90f)); // We use 90 since 45 is half of 90.


        // Position of icon using the mapped angle
        float iconX = mappedAngle * trackingArea.rect.width - halfWidth;

        // Set the position
        return new Vector2(iconX, rectTransform.anchoredPosition.y);
    }

    private Vector2 GetXPosFromSourceForwardToTrackForward()
    {
        // Get the forward directions
        Vector3 rotatingSourceForward = -Vector3.ProjectOnPlane(rotatingSource.forward, Vector3.up);
        Vector3 pointToTrackForward = Vector3.ProjectOnPlane(pointToTrack.forward, Vector3.up);

        Debug.DrawRay(Vector3.zero + Vector3.up, pointToTrackForward.normalized * 2f, Color.yellow);
        Debug.DrawRay(Vector3.zero, rotatingSourceForward.normalized * 5f, Color.magenta);
        // Angle between the forward directions
        float angle = Vector3.SignedAngle(rotatingSourceForward, pointToTrackForward, Vector3.up);

        // Normalize to 0 - 1
        float normalizedAngle = 0.5f + angle / 360f;

        // Now the Icon goes from Right to left -> Indicating watching behaviour of the witch
        float reversedNormalizedAngle = 1 - normalizedAngle;

        // Position of icon
        float iconX = reversedNormalizedAngle * trackingArea.rect.width - halfWidth;

        // Set the position
        return new Vector2(iconX, rectTransform.anchoredPosition.y);
    }
}

