using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public bool ShowNotification = true;

    private bool activated = false;
    private Quaternion playerRotation;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Player") || activated == true)
            return;

        if (ShowNotification)
            StateManager.Instance.NewCheckpointEvent.Invoke();

        activated = true;
        playerRotation = player.transform.rotation;
        CheckpointManager.Instance.Checkpoint = this;
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    public Vector3 GetRespawnPoint()
    {
        return this.transform.position;
    }

    public Quaternion GetRotation()
    {
        return playerRotation;
    }
}
