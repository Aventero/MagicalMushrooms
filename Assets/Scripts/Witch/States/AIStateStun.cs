using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateStun : MonoBehaviour, IAIState
{
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    public AIStates StateName => AIStates.Stun;
    private float currentStunTime = 0f;
    public float MaxStunTime = 4f;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.Vision.SetWatchingMode(WatchingMode.Slow);
        stateManager.DangerOverlay.SetState(DangerState.Safe);
        stateManager.UIAnimation.PlayEyeClose();
        stateManager.ToggleWitchLocator(false);
        stateManager.VisionCone.SetActive(false);
        stateManager.Movement.StopAgent();
        currentStunTime = 0f;
    }

    public void UpdateState()
    {
        currentStunTime += Time.deltaTime;

        LookDown();

        if (currentStunTime > MaxStunTime)
        {
            stateManager.VisionCone.SetActive(true);
            stateManager.Vision.Watch(stateManager.StandardWatchpoint.transform.position);
            if (stateManager.Vision.HasReachedTarget())
                stateManager.TransitionToState(AIStates.PanicSearch);
        }
    }

    private void LookDown()
    {
        Vector3 groundWatchPoint = transform.position + transform.forward * 2f;
        stateManager.Watch(groundWatchPoint);
    }

    public void ExitState()
    {
        stateManager.VisionCone.SetActive(true);
    }
}
