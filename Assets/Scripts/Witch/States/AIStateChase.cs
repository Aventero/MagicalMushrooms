﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

internal class AIStateChase : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.Chase;
    public Transform ChasePoint;

    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    private AIVision vision;

    public float maxDistance = 100f; // Maximum distance to check from the starting position
    public float AgentChasingRotationSpeed = 5f;

    [Range(1, 5)]
    public float samplingForWalkPoint = 0.5f; // The distance between each sampled point

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
        vision = stateManager.Vision;
    }

    public void EnterState()
    {
        // UI
        stateManager.DangerOverlay.SetState(DangerState.Danger);
        stateManager.UIAnimation.PlayPupilExpand(vision.AttackAfterSeconds, false);
        stateManager.ToggleWitchLocator(true);

        // Watching
        vision.SetWatchingMode(WatchingMode.Chasing);
        stateManager.Watch(stateManager.Player.position);

        // Chase player
        stateManager.Movement.agent.updateRotation = false; // !! This makes the agent not rotate on its own. !!
        ChasePoint.position = stateManager.Player.position;
        stateManager.Movement.SetWalkPoint(GetClosestPointNearPlayerOnLine(stateManager.Player.position));

        stateManager.Movement.StartAgent();
    }

    public void ExitState()
    {
        stateManager.Movement.agent.updateRotation = true;  // !! Agent rotate on its own. !!
        stateManager.Movement.StopAgent();
        stateManager.ToggleWitchLocator(false);
    }

    public void UpdateState()
    {
        stateManager.Watch(stateManager.Player.position);
        stateManager.Vision.RotateAgent(stateManager.Player.position, AgentChasingRotationSpeed);
        
        //stateManager.SetWalkPoint(stateManager.Player.position); // TODO: Dont let her run after the player!!
        if (stateManager.HasLostPlayer())
        {
            // Has Lost the player
            stateManager.TransitionToState(AIStates.LostPlayer);
            return;
        }
        else
        {
            // Is chasing
            vision.ChaseTime += Time.deltaTime;
            if (vision.ChaseTime >= vision.AttackAfterSeconds)
            {
                vision.ChaseTime = 0f;
                stateManager.Movement.agent.isStopped = true;
                stateManager.TransitionToState(AIStates.Capture);
            }
        }
    }


    public Vector3 GetClosestPointNearPlayerOnLine(Vector3 playerPosition)
    {
        Vector3 directionFromPlayer = (transform.position - playerPosition).normalized;
        Vector3 currentSamplePoint = playerPosition;  // Starting close to the player
        float distance = Vector3.Distance(playerPosition, currentSamplePoint);

        while (distance < maxDistance)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(currentSamplePoint, out hit, 5.0f, NavMesh.AllAreas))
            {
                // Return the closest pos on navMesh
                Debug.DrawLine(transform.position, playerPosition, Color.yellow, 2f);
                return hit.position;
            }

            currentSamplePoint += directionFromPlayer * samplingForWalkPoint;
            distance = Vector3.Distance(playerPosition, currentSamplePoint);
        }
        // Nothing found, just use the player
        return playerPosition;
    }

    private bool AgentReachedDestination()
    {
        if (!stateManager.Movement.agent.pathPending && stateManager.Movement.agent.remainingDistance < stateManager.Movement.agent.stoppingDistance)
            return true;

        return false;
    }
}