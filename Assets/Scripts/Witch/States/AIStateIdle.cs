using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStateIdle : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.Idle;
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;

    public float WaitTimeInBetween = 2.0f;
    public int PointsToWatch = 2;
    [Range(0f, 1f)]
    public float ProbabilityOfTurning = 0.5f;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.DangerOverlay.SetState(DangerState.Nothing);
        stateManager.Vision.SetWatchingMode(WatchingMode.Slow);
        stateManager.Movement.StopAgent();
        List<Transform> visiblePointsAroundPlayer = stateManager.VisiblePointsAroundPlayer(transform.position, transform.forward, 75f);
        StartCoroutine(LookAround(WaitTimeInBetween, visiblePointsAroundPlayer));
    }

    public void ExitState()
    {
        StopAllCoroutines();
        stateManager.Movement.StartAgent();
    }

    public void UpdateState()
    {
        if (stateManager.HasFoundPlayer())
        {
            StopCoroutine(nameof(LookAround));
            stateManager.TransitionToState(AIStates.SpottetPlayer);
        }
    }

    IEnumerator LookAround(float waitTimeInBetween, List<Transform> visiblePoints)
    {
        // First keep watching the current point for a bit
        stateManager.Watch(stateManager.Vision.HeadWatchTarget.transform.position);
        yield return new WaitUntil(() => stateManager.Vision.HasReachedTarget());

        // Shuffle the list of visible points
        for (int i = 0; i < visiblePoints.Count; i++)
        {
            Transform temp = visiblePoints[i];
            int randomIndex = Random.Range(i, visiblePoints.Count);
            visiblePoints[i] = visiblePoints[randomIndex];
            visiblePoints[randomIndex] = temp;
        }

        int times = 0;
        foreach (Transform point in visiblePoints)
        {
            if (times >= PointsToWatch)
                break; // Return the Coroutine

            // Store previous, set the new one
            stateManager.Watch(point.position);
            yield return new WaitUntil(() => stateManager.Vision.HasReachedTarget());
            Debug.Log("Reached");
            times++;
            yield return new WaitForSeconds(waitTimeInBetween);
        }

        stateManager.TransitionToState(AIStates.Patrol);

        yield return null;
    }

    IEnumerator TurnAround()
    {
        yield return null;
    }
}
