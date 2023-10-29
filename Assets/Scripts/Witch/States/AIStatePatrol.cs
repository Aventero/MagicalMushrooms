using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AIStatePatrol : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.Patrol;
    public AIStateManager AIStateManager => stateManager;

    private AIStateManager stateManager;
    private Transform patrolWatchPoint;
    private bool completedWalk;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        PrepareForPatrol();
        SetPatrolWatchPoint();
        StartCoroutine(TurnThenWalk());
    }

    public void ExitState()
    {
        StopAllCoroutines();
    }

    public void UpdateState()
    {
        if (stateManager.HasFoundPlayer())
        {
            stateManager.TransitionToState(AIStates.SpottetPlayer);
            return;
        }

        if (completedWalk)
        {
            stateManager.TransitionToState(AIStates.Idle);
        }
    }

    private void PrepareForPatrol()
    {
        completedWalk = false;
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Vision.SetWatchingMode(WatchingMode.Slow);
        stateManager.Movement.StopAgent();
        //Transform walkPoint = stateManager.Movement.FindNewWalkpoint();
        //stateManager.Movement.SetWalkPoint(walkPoint.position);
        stateManager.Movement.MoveToNextPoint();
    }

    private void SetPatrolWatchPoint()
    {
        patrolWatchPoint = DetermineWatchPoint();
        Debug.DrawLine(stateManager.Vision.ViewCone.transform.position, patrolWatchPoint.position, Color.green, 2f);
    }

    private Transform DetermineWatchPoint()
    {
        Vector3 forwardToWalkpoint = stateManager.Movement.currentWalkPoint - transform.position;
        List<Transform> visiblePoints = stateManager.VisiblePointsAroundPlayer(stateManager.Movement.currentWalkPoint, forwardToWalkpoint, 75f);

        if (visiblePoints.Count == 0)
        {
            return stateManager.StandardWatchpoint.transform;
        }

        return visiblePoints[Random.Range(0, visiblePoints.Count)];
    }

    private bool HeadHasToTurn()
    {
        return stateManager.EasyAngle(transform.position, transform.forward, patrolWatchPoint.position) > 75f;
    }

    private bool ReachedDestination()
    {
        return !stateManager.Movement.agent.pathPending && stateManager.Movement.agent.remainingDistance < stateManager.Movement.agent.stoppingDistance;
    }

    private IEnumerator TurnThenWalk()
    {
        //stateManager.Watch(HeadHasToTurn() ? stateManager.StandardWatchpoint.transform.position : patrolWatchPoint.position);
        //yield return new WaitUntil(() => stateManager.Vision.ReachedWatchTarget);
        yield return null;
        stateManager.Movement.StartAgent();
        StartCoroutine(WalkWatching());
    }

    private IEnumerator WalkWatching()
    {
        while (!ReachedDestination())
        {
            WatchBasedOnConditions();
            yield return null;
        }

        FinalizePatrolWalk();
    }

    private void WatchBasedOnConditions()
    {
        if (HeadHasToTurn() || !stateManager.Vision.HasReachedTarget())
        {
            stateManager.Watch(stateManager.StandardWatchpoint.transform.position);
        }
        else
        {
            Debug.DrawLine(stateManager.Vision.ViewCone.transform.position, stateManager.Vision.currentWatchTarget, Color.cyan);
            stateManager.Watch(patrolWatchPoint.position);
        }
    }

    private void FinalizePatrolWalk()
    {
        stateManager.Movement.StopAgent();
        //stateManager.Vision.SetWatchingMode(WatchingMode.Relaxed);
        //stateManager.Watch(patrolWatchPoint.position);
        //StartCoroutine(WatchFinalPatrolpoint());
        completedWalk = true;
    }

    private IEnumerator WatchFinalPatrolpoint()
    {
        yield return new WaitUntil(() => stateManager.Vision.HasReachedTarget());
        completedWalk = true;
    }
}