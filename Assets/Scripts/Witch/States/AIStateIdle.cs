using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIdle : MonoBehaviour, AIState
{
    public string StateName => "Idle";
    public float WaitTimeInBetween = 2.0f;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.DangerBlit.SetState(DangerState.Nothing);
        stateManager.aiVision.RelaxedWatching();
        stateManager.agent.isStopped = true;
        List<Transform> visiblePoints = stateManager.CalculateVisiblePoints(transform.position, transform.forward, 75f);
        StartCoroutine(LookAround(stateManager, WaitTimeInBetween, visiblePoints));
    }

    public void ExitState(AIStateManager stateManager)
    {
        StopAllCoroutines();
        stateManager.agent.isStopped = false;
    }

    public void UpdateState(AIStateManager stateManager)
    {
        if (stateManager.HasFoundPlayer())
        {
            StopCoroutine(nameof(LookAround));
            stateManager.TransitionToState("Chase");
        }
    }

    IEnumerator LookAround(AIStateManager stateManager, float waitTimeInBetween, List<Transform> visiblePoints)
    {
        // First keep watching the current point for a bit
        stateManager.Watch(stateManager.aiVision.CurrentWatchTarget.transform.position);
        yield return new WaitForSeconds(waitTimeInBetween / 2f);

        int times = 0;
        foreach (Transform point in visiblePoints)
        {
            if (times >= 3)
                break; // Return the Coroutine

            // Store previous, set the new one
            stateManager.Watch(point);
            times++;

            yield return new WaitForSeconds(waitTimeInBetween);
        }

        stateManager.TransitionToState("Patrol");
        yield return null;
    }
}
