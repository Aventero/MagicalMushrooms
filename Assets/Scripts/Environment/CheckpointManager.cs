using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private CharacterController controller;

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

    // This respawns the player at the last checkpoint
    public void RespawnPlayer()
    {
        if(Checkpoint == null)
        {
            // TODO: Do something if the checkpoint has not being set
            Debug.LogError("No checkpoint!");
            return;
        }


        controller.enabled = false;

        // The player can only be teleported if the CharacterController is disabled 
        controller.transform.SetPositionAndRotation(Checkpoint.GetRespawnPoint(), Checkpoint.GetRotation());
        controller.enabled = true;
    }
}
