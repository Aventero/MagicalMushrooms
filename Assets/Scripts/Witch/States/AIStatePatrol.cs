using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AIStatePatrol :  MonoBehaviour, AIState
{
    public string StateName => "Patrol";
    private Transform patrolWatchPoint;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.aiVision.RelaxedWatching();
        stateManager.StopAgent();
        Transform walkPoint = stateManager.FindNewWalkpoint();
        stateManager.SetWalkPoint(walkPoint.position);

        // Find a point to watch
        Vector3 forwardToWalkpoint = stateManager.currentWalkPoint - transform.position;
        List<Transform> visiblePointsAtNextDestination = stateManager.CalculateVisiblePoints(stateManager.currentWalkPoint, forwardToWalkpoint, 75f);

        // No Visible found or Its behind the witch -> Just watch foward
        if (visiblePointsAtNextDestination.Count == 0)
        {
            Debug.Log("No visible point found!");
            patrolWatchPoint = stateManager.StandardWatchpoint.transform;
        }
        else
            patrolWatchPoint = visiblePointsAtNextDestination[UnityEngine.Random.Range(0, visiblePointsAtNextDestination.Count)];


        // If the patrolWatchPoint is behind the witch watch, dont use it
        if (stateManager.EasyAngle(transform.position, transform.forward, patrolWatchPoint.position) > 75f)
        {
            patrolWatchPoint = stateManager.StandardWatchpoint.transform;
        }

        stateManager.Watch(patrolWatchPoint);
        stateManager.Walk();

    }

    public void ExitState(AIStateManager stateManager)
    {
    }


    public void UpdateState(AIStateManager stateManager)
    {
        stateManager.Watch(patrolWatchPoint);

        if (stateManager.HasFoundPlayer())
        {
            stateManager.TransitionToState("Chase");
            return;
        }

        // check if reached a walkpoint
        if (!stateManager.agent.pathPending && stateManager.agent.remainingDistance < stateManager.agent.stoppingDistance)
        {
            stateManager.TransitionToState("Idle");
        }
    }

    private bool IsTurning(AIStateManager stateManager)
    {
        if (stateManager.EasyAngle(transform.position, transform.forward, patrolWatchPoint.position) > 75f)
        {
            Debug.Log(stateManager.EasyAngle(transform.position, transform.forward, patrolWatchPoint.position));
            return true;
        }
        return false;
    }
}