using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

internal class AIStateChase : MonoBehaviour, AIState
{
    public string StateName => "Chase";
    private NavMeshAgent agent; 
    public float AttackAfterSeconds = 1f;
    private float chaseTime = 0f;
    public Transform ChasePoint;

    [ColorUsageAttribute(true, true)]
    public Color Chasing;

    [ColorUsageAttribute(true, true)]
    public Color PlayerSafe;


    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        chaseTime = 0f;
        stateManager.aiVision.PlayerWatching();
        stateManager.SetBlitColor(Chasing);
        stateManager.LerpBlit(0.2f, stateManager.BlitTime, true);
        agent = stateManager.agent;
        stateManager.Watch(stateManager.Player);
        ChasePoint.position = stateManager.Player.position;
        stateManager.SetWalkPoint(ChasePoint.position);
        stateManager.Walk();
    }

    public void ExitState(AIStateManager stateManager)
    {
    }

    public void UpdateState(AIStateManager stateManager)
    {
        stateManager.Watch(stateManager.Player);

        if (stateManager.HasLostPlayer())
        {
            stateManager.SetBlitColor(PlayerSafe);
            stateManager.LerpBlit(0f, stateManager.BlitTime, false);
            stateManager.TransitionToState("LostPlayer");
            return;
        }

        chaseTime += Time.deltaTime;
        if (AgentReachedDestination(stateManager) && chaseTime >= AttackAfterSeconds)
        {
            chaseTime = 0f;
            agent.isStopped = true;
            stateManager.TransitionToState("Attack");
        }
    }

    private bool AgentReachedDestination(AIStateManager stateManager)
    {
        if (!stateManager.agent.pathPending && stateManager.agent.remainingDistance < stateManager.agent.stoppingDistance)
            return true;

        return false;
    }
}