using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateSpottedPlayer : MonoBehaviour, IAIState
{
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    public AIStates StateName => AIStates.SpottetPlayer;
    public float SpottedDuration = 1f;
    private float spottingTimer = 0f;
    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        spottingTimer = 0;
        stateManager.DangerOverlay.SetState(DangerState.Danger);
        stateManager.UIAnimation.PlayEyeOpen(SpottedDuration);
    }

    public void UpdateState()
    {
        spottingTimer += Time.deltaTime;

        if (spottingTimer >= SpottedDuration)
            stateManager.TransitionToState(AIStates.Chase);

        if (stateManager.aiVision.LostPlayer())
            stateManager.TransitionToState(AIStates.LostPlayer);
    }

    public void ExitState()
    {
        spottingTimer = 0;
    }
}
