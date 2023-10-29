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
        stateManager.UIAnimation.PlayEyeClose();
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Movement.agent.isStopped = true;
        stateManager.Vision.SetWatchingMode(WatchingMode.Slow);
        List<Transform> visiblePoints = stateManager.VisiblePointsAroundPlayer(transform.position, transform.forward, 75f);
        StartCoroutine(LookAround(WaitTimeInBetween, visiblePoints));
    }

    public void ExitState()
    {
        StopAllCoroutines();
        stateManager.Movement.agent.isStopped = false;
    }

    public void UpdateState()
    {
    }

    IEnumerator LookAround(float waitTimeInBetween, List<Transform> visiblePoints)
    {
        int times = 0;
        foreach (Transform point in visiblePoints)
        {
            if (times >= 1)
                break; // Return the Coroutine

            // Store previous, set the new one
            stateManager.Watch(point.position);
            times++;

            yield return new WaitForSeconds(waitTimeInBetween);
        }

        stateManager.TransitionToState(AIStates.Patrol);
        yield return null;
    }
}
