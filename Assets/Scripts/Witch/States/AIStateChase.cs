using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal class AIStateChase : MonoBehaviour, AIState
{
    public string StateName => "Chase";
    private NavMeshAgent agent; 
    public float AttackAfterSeconds = 1f;
    private float chaseTime = 0f;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        chaseTime = 0f;
        stateManager.aiVision.PlayerWatching();
        //stateManager.animator.SetBool("Stay", false);
        agent = stateManager.agent;
       
        stateManager.Watch(stateManager.Player);
    }

    public void ExitState(AIStateManager stateManager)
    {
        //stateManager.animator.SetBool("Stay", true);
    }

    public void UpdateState(AIStateManager stateManager)
    {
        agent.destination = stateManager.Player.position;
        chaseTime += Time.deltaTime;
        if (chaseTime >= AttackAfterSeconds)
        {
            chaseTime = 0f;
            agent.isStopped = true;
            stateManager.TransitionToState("Attack");
        }
    }
}