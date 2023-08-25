using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Checkpoint Checkpoint { get; set; }

    private void Start()
    {
        StateManager.Instance.RespawnPlayerEvent.AddListener(respawnPlayerEvent);
    }

    public void respawnPlayerEvent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localPosition = Checkpoint.getRespawnPoint();
    }
}
