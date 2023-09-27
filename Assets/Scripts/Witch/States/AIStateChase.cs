using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

internal class AIStateChase : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.Chase;
    private NavMeshAgent agent; 

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
        agent = stateManager.agent;
        vision = stateManager.aiVision;
    }

    public void EnterState()
    {
        // UI
        stateManager.DangerOverlay.SetState(DangerState.Danger);
        stateManager.UIAnimation.PlayPupilExpand(vision.AttackAfterSeconds, false);
        
        // Watching
        vision.SetWatchingMode(WatchingMode.Chasing);
        stateManager.Watch(stateManager.Player);

        // Chase player
        agent.updateRotation = false; // !! This makes the agent not rotate on its own. !!
        ChasePoint.position = stateManager.Player.position;
        stateManager.SetWalkPoint(GetClosestPointNearPlayerOnLine(stateManager.Player.position));

        stateManager.Walk();
    }

    public void ExitState()
    {
        agent.updateRotation = true;  // !! Agent rotate on its own. !!
    }

    public void UpdateState()
    {
        stateManager.Watch(stateManager.Player);

        RotateAgent(stateManager.Player.position);
        
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
                agent.isStopped = true;
                stateManager.TransitionToState(AIStates.Capture);
            }
        }
    }


    // Use this for rotations????? Whenever looking at a new pooint?
    private void RotateAgent(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Do not rotate upwards or downwards
        directionToTarget.y = 0;

        if (directionToTarget == Vector3.zero)
        {
            return; // Avoid trying to look in a zero direction
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AgentChasingRotationSpeed * Time.deltaTime);

        // Set the agent's desired velocity so it moves in the direction we set.
        agent.velocity = transform.forward * agent.speed;
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
                return hit.position;
            }

            currentSamplePoint += directionFromPlayer * samplingForWalkPoint;
            distance = Vector3.Distance(playerPosition, currentSamplePoint);
        }
        Debug.DrawLine(transform.position, playerPosition, Color.red, 5f);
        return playerPosition;
    }

    private bool AgentReachedDestination()
    {
        if (!stateManager.agent.pathPending && stateManager.agent.remainingDistance < stateManager.agent.stoppingDistance)
            return true;

        return false;
    }
}