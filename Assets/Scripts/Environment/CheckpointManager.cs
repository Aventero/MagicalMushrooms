using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private CharacterController controller;

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
        StateManager.Instance.RespawnPlayerEvent.AddListener(RespawnPlayer);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<CharacterController>();
    }

    public void RespawnPlayer()
    {
        controller.enabled = false;

        // The player can only be teleported if the CharacterController is disabled 
        controller.transform.SetPositionAndRotation(Checkpoint.GetRespawnPoint(), Checkpoint.GetRotation());

        controller.enabled = true;
    }
}
