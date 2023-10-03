using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AIStateTurn : MonoBehaviour, IAIState
{
	public AIStates StateName => AIStates.Patrol;
	public AIStateManager AIStateManager => stateManager;

	private AIStateManager stateManager;
    private Vector3 PlayerPositionOnEntering;
    public float RotationSpeed = 2f;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
   
    }
    public void EnterState()
    {
        stateManager.Vision.SetWatchingMode(WatchingMode.Relaxed);
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Movement.StopAgent();
        stateManager.Movement.agent.updateRotation = false; // Let me do the rotation!
        PlayerPositionOnEntering = stateManager.Player.position;
        stateManager.Vision.Watch(stateManager.StandardWatchpoint.transform.position);
    }

    public void UpdateState()
    {
        stateManager.Vision.Watch(stateManager.StandardWatchpoint.transform.position);
        if (stateManager.Vision.RotateAgent(PlayerPositionOnEntering, RotationSpeed))
        {
            stateManager.TransitionToState(AIStates.Idle);
        }

        if (stateManager.HasFoundPlayer())
        {
            stateManager.TransitionToState(AIStates.SpottetPlayer);
        }
    }


    public void ExitState()
    {
        stateManager.Movement.agent.updateRotation = true; // Agent can rotate on its own.
    }

}