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

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, draggingLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.white);
                if (hit.collider.CompareTag("Draggable"))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                    IsDragging = true;
                    draggingObject = hit.collider.gameObject;
                    draggingBody = hit.collider.GetComponent<Rigidbody>();
                }
            }
        }

        // Dragging the object
        if (IsDragging && draggingObject != null)
        {
            //draggingBody.isKinematic = true;
            draggingBody.interpolation = RigidbodyInterpolation.Interpolate;

            // Calculate target position
            Vector3 cursorPosition = Input.mousePosition;
            cursorPosition.z = DistanceFromCamera;

            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(cursorPosition);
            draggingBody.velocity = (targetPosition - draggingBody.position) * 10f;
        }

        if (IsDragging && Input.GetMouseButtonUp(0))
        {
            draggingBody.interpolation = RigidbodyInterpolation.None;
            draggingBody.isKinematic = false;
            IsDragging = false;
            draggingObject = null;
            draggingBody = null;
        }

    }
}
