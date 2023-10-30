using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStateAlert : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.Alert;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    public float MaxWatchTime = 3f;
    public float currentWatchTime = 0f;
    public bool pointOfInterestReached = false;


    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.WitchQuestionSign.SetActive(true);
        stateManager.VisionScaling = -30f;
        StateManager.Instance.IsAllowedToSeePlayer = false;
        Vector3 dirToPlayer = (stateManager.Player.position - transform.position).normalized;
        stateManager.Movement.SetWalkPoint(transform.position + dirToPlayer * 2.5f);
        stateManager.Movement.StartAgent();
        stateManager.Vision.Watch(stateManager.PointOfInterest);
        stateManager.UIAnimation.PlayEyeClose();
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Vision.SetWatchingMode(WatchingMode.Slow);
        currentWatchTime = 0f;
        pointOfInterestReached = false;
    }

    public void ExitState()
    {
        stateManager.WitchQuestionSign.SetActive(false);
        StateManager.Instance.IsAllowedToSeePlayer = true;
        stateManager.VisionScaling = 0f;
    }

    public void UpdateState()
    {
        // Movement
        if (ReachedDestination())
            stateManager.Movement.StopAgent();

        if (stateManager.Vision.HasReachedTarget() && !pointOfInterestReached)
        {
            StateManager.Instance.IsAllowedToSeePlayer = true;
            pointOfInterestReached = true;
            stateManager.VisionScaling = 0f;
        }

        if (pointOfInterestReached)
        {
            currentWatchTime += Time.deltaTime;

            if (currentWatchTime >= MaxWatchTime)
                stateManager.TransitionToState(AIStates.Patrol);
        }

        if (stateManager.HasFoundPlayer() && pointOfInterestReached)
            stateManager.TransitionToState(AIStates.SpottetPlayer);
    }

    private bool ReachedDestination()
    {
        return !stateManager.Movement.agent.pathPending && stateManager.Movement.agent.remainingDistance < stateManager.Movement.agent.stoppingDistance;
    }
}
