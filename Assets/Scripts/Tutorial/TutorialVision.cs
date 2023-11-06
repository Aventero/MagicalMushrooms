using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialVision : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Ray ray = new Ray(transform.position, (other.transform.position - transform.position).normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    CheckpointManager.Instance.RespawnPlayer();
                }
            }
        }
    }
}
