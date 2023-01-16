using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIgnorePlayerIdle : MonoBehaviour, AIState
{
    public string StateName => "IgnorePlayerIdle";
    public float WaitTimeInBetween = 2.0f;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.agent.isStopped = true;
        stateManager.aiVision.RelaxedWatching();
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
    }

    IEnumerator LookAround(AIStateManager stateManager, float waitTimeInBetween, List<Transform> visiblePoints)
    {
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
