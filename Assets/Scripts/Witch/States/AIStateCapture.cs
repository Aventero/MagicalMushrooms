using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AIStateCapture : MonoBehaviour, IAIState
{
    public GameObject CagePrefab;

    [Header("Height of player from 0 to 2 while 1 is the destination")]
    public AnimationCurve captureMotion;
    public float CageHeight = 2f;
    public float CaptureSpeed = 0.5f;

    private Transform playerTransform;
    private Vector3 startPosition;
    private GameObject spawnedCage;
    private float captureTimer = 0f;
    private float percentCaptured = 0f;
    private bool isCapturing = false;

    public string StateName => "Capture";
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;


    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
        playerTransform = stateManager.Player;
    }

    public void EnterState()
    {
        captureTimer = 0f;
        percentCaptured = 0f;
        // Spawn Cage
        spawnedCage = Instantiate(CagePrefab, playerTransform.position + Vector3.up * CageHeight, Quaternion.identity);
        isCapturing = true;
        StateManager.Instance.PauseMovementEvent.Invoke();
        spawnedCage.transform.position = playerTransform.position + Vector3.up;
        startPosition = playerTransform.position;
    }

    public void ExitState()
    {
        // Delete Cage
        if (spawnedCage)
        {
            Destroy(spawnedCage);
        }
        isCapturing = false;
        StateManager.Instance.ResumeMovementEvent.Invoke();
        StateManager.Instance.RespawnPlayerEvent.Invoke();
    }

    public void UpdateState()
    {
        if (!isCapturing)
            return;

        // Hover Cage above player

        // Cage has a point where after some time the player will be pulled to
        captureTimer += Time.deltaTime * CaptureSpeed;
        percentCaptured = captureTimer / CageHeight;

        // Dragging Up
        //percentCaptured = Mathf.SmoothStep(0, 1, percentCaptured); // Smooth at the ends
        playerTransform.position = Vector3.Lerp(startPosition, spawnedCage.transform.position + Vector3.up, Mathf.InverseLerp(0f, 2f, captureMotion.Evaluate(percentCaptured)));
        //playerTransform.position = Vector3.MoveTowards(playerTransform.position, spawnedCage.transform.position, Time.deltaTime * captureSpeed);

        // Player Reached Destinaion
        if (percentCaptured >= 1f)
        {
            CloseCageGate();
            // Fade screen
            stateManager.TransitionToState("Patrol");
        }
        

    }

    private void CloseCageGate()
    {
        // spawnedCage.GetComponent<CageController>().CloseGate();
    }
}