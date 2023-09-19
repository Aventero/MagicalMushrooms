using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour
{
    public LayerMask clippingAvoidanceLayerMask;
    public LayerMask draggingLayerMask;

    [ReadOnly]
    public GameObject draggingObject = null;
    private Rigidbody draggingBody = null;
    [ReadOnly]
    public bool IsDragging = false;
    public float DistanceFromCamera = 3.0f;
    public float DraggingSpeed = 2.0f;
    public float AvoidanceDistance = 1f;
    public float MaxVelocity = 10f;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, Mathf.Infinity, draggingLayerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.white);
            if (hit.collider.CompareTag("Draggable") && hit.collider.GetComponent<Rigidbody>() != null)
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                IsDragging = true;
                draggingObject = hit.collider.gameObject;
                draggingBody = hit.collider.GetComponent<Rigidbody>();
            }
        }

        // Dragging the object
        if (IsDragging && draggingObject)
        {
            draggingBody.isKinematic = false;
            draggingBody.interpolation = RigidbodyInterpolation.Interpolate;

            // Calculate target position
            Vector3 cursorPosition = Input.mousePosition;
            cursorPosition.z = DistanceFromCamera;

            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(cursorPosition);
            Vector3 velocity = (targetPosition - draggingBody.position) * 10f;
            velocity.x = Mathf.Clamp(velocity.x, -MaxVelocity, MaxVelocity);
            velocity.y = Mathf.Clamp(velocity.y, -MaxVelocity, MaxVelocity);
            velocity.z = Mathf.Clamp(velocity.z, -MaxVelocity, MaxVelocity);
            draggingBody.velocity = velocity;
        }

        if (IsDragging && draggingObject != null && Input.GetMouseButton(1))
        {
            draggingBody.transform.rotation = Quaternion.identity;
            draggingBody.rotation = Quaternion.identity;
            draggingBody.angularVelocity = Vector3.zero;
            Debug.Log("Right Mouse button on draggign");
        }

        if (IsDragging && Input.GetMouseButtonDown(0))
        {
            Outline outline = draggingBody.gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.enabled = true;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 1;

        }

        if (IsDragging && Input.GetMouseButtonUp(0))
        {
            Outline outline = draggingBody.GetComponent<Outline>();
            outline.enabled = false;
            Destroy(outline);
            draggingBody.interpolation = RigidbodyInterpolation.None;
            draggingBody.isKinematic = false;
            IsDragging = false;
            draggingObject = null;
            draggingBody = null;
        }

    }
}
