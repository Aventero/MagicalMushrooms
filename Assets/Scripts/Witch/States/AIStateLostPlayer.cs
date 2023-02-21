using UnityEngine;

public class AIStateLostPlayer : MonoBehaviour, AIState
{
    public string StateName => "LostPlayer";
    public float WatchingTime = 2f;
    private float watchingTimer = 0f;

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.Watch(stateManager.Player.position);
        stateManager.agent.isStopped = true;
    }

    public void ExitState(AIStateManager stateManager)
    {
        watchingTimer = 0;
    }

    public void InitState(AIStateManager stateManager)
    {
    }

    public void UpdateState(AIStateManager stateManager)
    {
        watchingTimer += Time.deltaTime;

        // Get back to normal
        if (watchingTimer >= WatchingTime)
        {
            stateManager.TransitionToState("Idle");
            return;
        }

        // Found the player again
        if (stateManager.HasFoundPlayer())
            stateManager.TransitionToState("Chase");
    }
}
