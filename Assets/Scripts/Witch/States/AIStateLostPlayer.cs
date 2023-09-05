using UnityEngine;

public class AIStateLostPlayer : MonoBehaviour, IAIState
{
    public string StateName => "LostPlayer";
    public float WatchingTime = 2f;
    private float watchingTimer = 0f;
    public float SafeBlitTime = 1f;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.Watch(stateManager.Player.position);
        stateManager.agent.isStopped = true;
        stateManager.DangerBlit.SetState(DangerState.Safe);
    }

    public void ExitState()
    {
        watchingTimer = 0;
    }

    public void UpdateState()
    {
        watchingTimer += Time.deltaTime;

        // Get back to normal
        if (watchingTimer >= WatchingTime)
        {
            stateManager.TransitionToState("Levitate");
            return;
        }

        // Found the player again
        if (stateManager.HasFoundPlayer())
            stateManager.TransitionToState("Chase");
    }
}
