using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal class AIStateChase : MonoBehaviour, AIState
{
    public string StateName => "Chase";
    private NavMeshAgent agent;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.aiVision.PlayerWatching();
        stateManager.animator.SetBool("Stay", false);
        agent = stateManager.agent;
       
        stateManager.Watch(stateManager.Player);
    }

    public void ExitState(AIStateManager stateManager)
    {
        stateManager.animator.SetBool("Stay", true);
    }

    public void UpdateState(AIStateManager stateManager)
    {
        agent.destination = stateManager.Player.position;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    agent.isStopped = true;
                    stateManager.TransitionToState("Attack");
                }
            }
        }
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
    }
}