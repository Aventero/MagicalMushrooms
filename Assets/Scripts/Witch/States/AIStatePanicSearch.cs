using System.Collections;
using UnityEngine;

public class AIStatePanicSearch : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.PanicSearch;

    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;
    private Coroutine searchRoutine;
    public float SearchRadius;
    public float LookDuration;
    public int Looks;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.DangerOverlay.SetState(DangerState.Safe);
        stateManager.Vision.SetWatchingMode(WatchingMode.Paniced);
        stateManager.Movement.agent.isStopped = true;

        if (searchRoutine != null)
            StopCoroutine(searchRoutine);
        searchRoutine = StartCoroutine(FranticSearch());
    }

    public void ExitState()
    {
        if (searchRoutine != null)
            StopCoroutine(searchRoutine);
    }

    public void UpdateState()
    {
        // Found the player again
        if (stateManager.HasFoundPlayer())
            stateManager.TransitionToState(AIStates.SpottetPlayer);
    }

    IEnumerator FranticSearch()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * SearchRadius;
            Vector3 lookPoint = stateManager.Player.position + randomDirection;
            lookPoint.y = stateManager.Player.position.y;

            stateManager.Watch(lookPoint);

            yield return new WaitForSeconds(LookDuration);
        }

        // Has not found player!
        stateManager.TransitionToState(AIStates.Idle);
    }
}
