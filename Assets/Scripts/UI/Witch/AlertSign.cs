using UnityEngine;
using UnityEngine.UI;

public class AlertSign : MonoBehaviour
{
    public Vector2 offset;
    public RectTransform movedIconParent;
    public Transform tracking;


    private void Update()
    {
        // Convert the world position to viewport point
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(tracking.position + new Vector3(offset.x, offset.y, 0));

        // Check if the point is behind the camera
        if (viewportPoint.z < 0)
        {
            // Hide
            movedIconParent.gameObject.SetActive(false);
            return;
        }
        else
        {
            // Show
            movedIconParent.gameObject.SetActive(true);
        }

        Vector2 screenPoint = new Vector2(viewportPoint.x * Screen.width, viewportPoint.y * Screen.height);
        movedIconParent.position = screenPoint;
    }

}
