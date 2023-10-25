using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public string targetTag = "Draggable"; 
    public float radius = 5f; 
    public float LevitationTime = 3f;
    public float LevitationHeight = 2f;
    Collider[] colliders;

    private void OnEnable()
    {
        Vector3 center = transform.position;
        colliders = Physics.OverlapSphere(center, radius);

        // Iterate through the colliders and check their tags
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(targetTag))
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Levitate levitate = rb.AddComponent<Levitate>();
                    levitate.enabled = false;
                    levitate.Initialize(LevitationHeight * Random.value);
                    levitate.isLevitating = true;
                    levitate.enabled = true;
                }
            }
        }

        StartCoroutine(StopLevitation());
    }

    IEnumerator StopLevitation()
    {
        yield return new WaitForSeconds(LevitationTime);
        foreach (Collider collider in colliders)
        {
            if (collider == null)
                continue;

            if (collider.CompareTag(targetTag))
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                    Destroy(rb.gameObject.GetComponent<Levitate>());
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
