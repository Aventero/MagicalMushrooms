using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public string targetTag = "Draggable"; // Replace "YourTag" with the tag you're looking for
    public float radius = 5f; // Adjust the radius as needed
    public float LevitationTime = 3f;
    public float Strength = 5f;
    public AnimationCurve heightCurve;
    Collider[] colliders;

    private void OnEnable()
    {
        Vector3 center = transform.position; // You can use any position as the center
        colliders = Physics.OverlapSphere(center, radius);

        // Iterate through the colliders and check their tags
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(targetTag))
            {
                // If the collider has the desired tag, you can access its components
                // Example: Get the Rigidbody component
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.transform.AddComponent<Levitate>();
                }
            }
        }
    }

    private void OnDisable()
    {
        // Iterate through the colliders and check their tags
        foreach (Collider collider in colliders)
        {
            if (collider == null)
                continue;

            if (collider.CompareTag(targetTag))
            {
                // If the collider has the desired tag, you can access its components
                // Example: Get the Rigidbody component
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Destroy(rb.gameObject.GetComponent<Levitate>());
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
