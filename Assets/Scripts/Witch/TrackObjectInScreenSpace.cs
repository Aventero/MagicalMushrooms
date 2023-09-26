using UnityEngine;
using UnityEngine.UI;

public class TrackObjectInScreenSpace : MonoBehaviour
{
    public Transform pointToTrack;
    public RectTransform trackingArea; 

    private RectTransform rectTransform;
    private float halfWidth;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        halfWidth = trackingArea.rect.width * 0.5f;
    }

    private void Update()
    {
        // camera towards the witch
        Vector3 toEnemy = pointToTrack.position - Camera.main.transform.position;

        // Project onto the camera's horizontal plane
        Vector3 projectedToTrackPoint = Vector3.ProjectOnPlane(toEnemy, Camera.main.transform.up);

        // Angle from -180 to 180
        float angle = Vector3.SignedAngle(Camera.main.transform.forward, projectedToTrackPoint, Camera.main.transform.up);

        // Normalize to 0 - 1
        float normalizedAngle = 0.5f + angle / 360f;

        // Now the Icon goes from Right to left -> Indicating watching behaviour of the witch
        float reversedNormalizedAngle = 1 - normalizedAngle;

        // Position of icon
        float iconX = reversedNormalizedAngle * trackingArea.rect.width - halfWidth;

        // Set the position
        rectTransform.anchoredPosition = new Vector2(iconX, rectTransform.anchoredPosition.y);
    }
}

