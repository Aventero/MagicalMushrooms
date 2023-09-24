using System.Collections;
using System.Collections.Generic;
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
        vision.SnappyWatching();
        stateManager.Watch(stateManager.Player);

        // Chase player
        ChasePoint.position = stateManager.Player.position;
        stateManager.Walk();
    }

    public void ExitState()
    {
    }

    public void UpdateState()
    {
        stateManager.Watch(stateManager.Player);    
        stateManager.SetWalkPoint(stateManager.Player.position); // TODO: Dont let her run after the player!!
        if (stateManager.HasLostPlayer())
        {
            // Has Lost the play
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

    private bool AgentReachedDestination()
    {
        if (!stateManager.agent.pathPending && stateManager.agent.remainingDistance < stateManager.agent.stoppingDistance)
            return true;

        return false;
    }
}