using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AIStatePatrol :  MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.Patrol;
    private Transform patrolWatchPoint;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Vision.SetWatchingMode(WatchingMode.Relaxed);
        stateManager.Movement.StopAgent();
        Transform walkPoint = stateManager.Movement.FindNewWalkpoint();
        stateManager.Movement.SetWalkPoint(walkPoint.position);

        // Find a point to watch
        Vector3 forwardToWalkpoint = stateManager.Movement.currentWalkPoint - transform.position;
        List<Transform> visiblePointsAtNextDestination = stateManager.CalculateVisiblePoints(stateManager.Movement.currentWalkPoint, forwardToWalkpoint, 75f);

        // No Visible found or Its behind the witch -> Just watch foward
        if (visiblePointsAtNextDestination.Count == 0)
        {
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
        stateManager.Movement.Walk();

    }

    public void ExitState()
    {
    }


    public void UpdateState()
    {
        stateManager.Watch(patrolWatchPoint);

        if (stateManager.HasFoundPlayer())
        {
            stateManager.TransitionToState(AIStates.SpottetPlayer);
            return;
        }

        // check if reached a walkpoint
        if (!stateManager.Movement.agent.pathPending && stateManager.Movement.agent.remainingDistance < stateManager.Movement.agent.stoppingDistance)
        {
            stateManager.TransitionToState(AIStates.Idle);
        }
    }

    private bool IsTurning()
    {
        if (stateManager.EasyAngle(transform.position, transform.forward, patrolWatchPoint.position) > 75f)
        {
            Debug.Log(stateManager.EasyAngle(transform.position, transform.forward, patrolWatchPoint.position));
            return true;
        }
        return false;
    }
}