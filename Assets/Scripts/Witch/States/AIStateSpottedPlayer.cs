using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateSpottedPlayer : MonoBehaviour, IAIState
{
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    public AIStates StateName => AIStates.SpottetPlayer;
    [Header("Time needed for the witch to SEE the player")]
    public float SpottedDuration = 0.5f;
    private float spottingTimer = 0f;
    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        AudioManager.Instance.Play("witchAlert");
        spottingTimer = 0;
        stateManager.WitchExclamationSign.SetActive(true);
        stateManager.Watch(stateManager.Player.position);
        stateManager.Vision.SetWatchingMode(WatchingMode.Fast);
        stateManager.DangerOverlay.SetState(DangerState.Danger);
        stateManager.UIAnimation.PlayEyeOpen(SpottedDuration);
        stateManager.ToggleWitchLocator(true);
    }

    public void UpdateState()
    {
        spottingTimer += Time.deltaTime;

        if (spottingTimer >= SpottedDuration)
            stateManager.TransitionToState(AIStates.Chase);

        if (stateManager.Vision.LostPlayer())
            stateManager.TransitionToState(AIStates.LostPlayer);
    }

    public void ExitState()
    {
        spottingTimer = 0;
        stateManager.ToggleWitchLocator(false);
        stateManager.WitchExclamationSign.SetActive(false);
    }
}
