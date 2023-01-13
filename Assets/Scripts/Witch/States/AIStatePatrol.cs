using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AIStatePatrol :  MonoBehaviour, AIState
{
    public string StateName => "Patrol";

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.aiVision.RelaxedWatching();
        stateManager.FindNewWalkpoint();
        StartCoroutine(stateManager.FindWatchpointForPatrol());
        stateManager.Walk();
        //stateManager.animator.SetBool("Stay", false);
        stateManager.agent.isStopped = false;
    }

    public void ExitState(AIStateManager stateManager)
    {
    }


    public void UpdateState(AIStateManager stateManager)
    {
        // check if reached a walkpoint
        if (!stateManager.agent.pathPending && stateManager.agent.remainingDistance < stateManager.agent.stoppingDistance)
        {
            stateManager.TransitionToState("Idle");
        }
    }
}