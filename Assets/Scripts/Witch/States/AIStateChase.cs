using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

internal class AIStateChase : MonoBehaviour, AIState
{
    public string StateName => "Chase";
    private NavMeshAgent agent; 
    public float AttackAfterSeconds;
    private float chaseTime = 0f;
    public Transform ChasePoint;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.DangerBlit.SetState(DangerState.Danger);
        chaseTime = 0f;
        stateManager.aiVision.PlayerWatching();
        agent = stateManager.agent;
        // Watch and chase the player
        stateManager.Watch(stateManager.Player);
        ChasePoint.position = stateManager.Player.position;
        stateManager.Walk();
    }

    public void ExitState(AIStateManager stateManager)
    {
    }

    public void UpdateState(AIStateManager stateManager)
    {
        stateManager.Watch(stateManager.Player);
        stateManager.SetWalkPoint(stateManager.Player.position);

        if (stateManager.HasLostPlayer())
        {
            // Has Lost the play
            stateManager.TransitionToState("LostPlayer");
            return;
        }
        else
        {
            // Is chasing
            chaseTime += Time.deltaTime;
            if (chaseTime >= AttackAfterSeconds)
            {
                chaseTime = 0f;
                agent.isStopped = true;
                stateManager.TransitionToState("Attack");
            }
        }
    }

    private bool AgentReachedDestination(AIStateManager stateManager)
    {
        if (!stateManager.agent.pathPending && stateManager.agent.remainingDistance < stateManager.agent.stoppingDistance)
            return true;

        return false;
    }
}