using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIgnorePlayerIdle : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.IgnorePlayerIdle;
    public float WaitTimeInBetween = 2.0f;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.Movement.MoveToRandomPoint();
        stateManager.UIAnimation.PlayEyeClose();
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Movement.agent.isStopped = false;
        stateManager.Vision.SetWatchingMode(WatchingMode.Slow);
    }

    public void ExitState()
    {
        StopAllCoroutines();
        stateManager.Movement.agent.isStopped = true;
    }

    public void UpdateState()
    {
        stateManager.Vision.Watch(stateManager.StandardWatchpoint.transform.position);

        if (ReachedDestination())
            stateManager.TransitionToState(AIStates.Idle);
    }

    private bool ReachedDestination()
    {
        return !stateManager.Movement.agent.pathPending && stateManager.Movement.agent.remainingDistance < stateManager.Movement.agent.stoppingDistance;
    }
}
