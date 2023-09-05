using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIgnorePlayerIdle : MonoBehaviour, IAIState
{
    public string StateName => "IgnorePlayerIdle";
    public float WaitTimeInBetween = 2.0f;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.DangerBlit.SetState(DangerState.Nothing);
        stateManager.agent.isStopped = true;
        stateManager.aiVision.RelaxedWatching();
        List<Transform> visiblePoints = stateManager.CalculateVisiblePoints(transform.position, transform.forward, 75f);
        StartCoroutine(LookAround(WaitTimeInBetween, visiblePoints));
    }

    public void ExitState()
    {
        StopAllCoroutines();
        stateManager.agent.isStopped = false;
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
            stateManager.Watch(point);
            times++;

            yield return new WaitForSeconds(waitTimeInBetween);
        }

        stateManager.TransitionToState("Patrol");
        yield return null;
    }
}
