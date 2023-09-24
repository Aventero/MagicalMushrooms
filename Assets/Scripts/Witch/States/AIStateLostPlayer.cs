using UnityEngine;

public class AIStateLostPlayer : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.LostPlayer;
    public float WatchingTime = 2f;
    private float watchingTimer = 0f;
    public float SafeBlitTime = 1f;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    private AIVision vision;
    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
        vision = stateManager.aiVision;
    }

    public void EnterState()
    {
        stateManager.Watch(stateManager.Player.position);
        stateManager.agent.isStopped = true;
        stateManager.DangerOverlay.SetState(DangerState.Safe);
        stateManager.UIAnimation.PlayPupilExpand(vision.AttackAfterSeconds, true);
    }

    public void ExitState()
    {
        watchingTimer = 0;
    }

    public void UpdateState()
    {
        watchingTimer += Time.deltaTime;

        // Decrease Chase time but not below 0
        if (vision.ChaseTime >= 0)
            vision.ChaseTime -= Time.deltaTime;

        if (vision.ChaseTime <= 0 && watchingTimer >= WatchingTime)
        {
            vision.ChaseTime = 0;
            stateManager.UIAnimation.PlayEyeClose();
            stateManager.TransitionToState(AIStates.Patrol);
        }

        // Found the player again
        if (stateManager.HasFoundPlayer())
            stateManager.TransitionToState(AIStates.Chase);
    }
}
