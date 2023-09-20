using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIdle : MonoBehaviour, IAIState
{
    public string StateName => "Idle";
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;

    public float WaitTimeInBetween = 2.0f;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        stateManager.DangerBlit.SetState(DangerState.Nothing);
        stateManager.aiVision.RelaxedWatching();
        stateManager.agent.isStopped = true;
        Vector3 directionToPlayer = stateManager.Player.position - transform.position;
        directionToPlayer.y = 0;
        StartCoroutine(SmoothRotateThenLookAround(directionToPlayer));
    }

    public void ExitState()
    {
        StopAllCoroutines();
        stateManager.agent.isStopped = false;
    }

    public void UpdateState()
    {
        if (stateManager.HasFoundPlayer())
        {
            StopCoroutine(nameof(LookAround));
            stateManager.TransitionToState("Chase");
        }
    }

    private IEnumerator SmoothRotateThenLookAround(Vector3 targetDirection)
    {
        // Watch closest point
        stateManager.Watch(stateManager.CalculateClosestNotVisiblePoint(transform.position, transform.forward));

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        float elapsedTime = 0f;
        float rotationDuration = 1.0f; // Adjust this value for rotation speed

        while (elapsedTime < rotationDuration)
        {
            stateManager.agent.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exactly the target rotation
        stateManager.agent.transform.rotation = targetRotation;
        stateManager.agent.SetDestination(transform.position);

        // Let Agent look at the watch points after turning
        List<Transform> visiblePoints = stateManager.CalculateVisiblePoints(transform.position, transform.forward, 75f);
        StartCoroutine(LookAround(stateManager, WaitTimeInBetween, visiblePoints));
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

        // Completed Looking back to Patrol!
        stateManager.TransitionToState("Patrol");
        yield return null;
    }
}
