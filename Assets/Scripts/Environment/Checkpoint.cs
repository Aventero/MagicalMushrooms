using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3 respawnPoint;
    private bool activated = false;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Player" || activated == true)
            return;

        Debug.Log("Setting Checkpoint!");
        activated = true;
        CheckpointManager.Instance.Checkpoint = this;
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    public Vector3 getRespawnPoint()
    {
        return respawnPoint;
    }
}
