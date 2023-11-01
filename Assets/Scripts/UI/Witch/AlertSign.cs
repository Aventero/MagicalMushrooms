using UnityEngine;
using UnityEngine.UI;

public class AlertSign : MonoBehaviour
{
    public Vector3 offset; // Offset for positioning
    public RectTransform movedIconParent;
    public Transform tracking;
    private Camera mainCamera; // Cached camera

    private void Start()
    {
        mainCamera = Camera.main;
    }


    private void Update()
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(tracking.position + offset);

        Debug.Log(viewportPoint.z);
        // Check if the tracked object is behind the camera
        if (viewportPoint.z <= 0)
        {
            // Hide the icon and exit the update if the object is behind the camera
            movedIconParent.gameObject.SetActive(false);
            return;
        }
        else
        {
            // Make sure the icon is visible if the object is in front of the camera
            movedIconParent.gameObject.SetActive(true);
        }

        // Keep the icon within the bounds of the screen
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, 0, 1);
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, 0, 1);

        // Convert the clamped viewport point back to screen space
        Vector2 screenPoint = new Vector2(viewportPoint.x * Screen.width, viewportPoint.y * Screen.height);

        // Ensure the movedIconParent's position is anchored inside the screen bounds
        screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width - movedIconParent.rect.width);
        screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height - movedIconParent.rect.height);

        movedIconParent.position = screenPoint;
    }
}
