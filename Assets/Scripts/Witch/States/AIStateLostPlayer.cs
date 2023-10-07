using System.Collections;
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

    // Lost the player
    private Coroutine searchRoutine;
    public float searchRadius = 5f;   // radius around the player's last known position
    public float lookDuration = 1f;   // how long to look in one direction
    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
        vision = stateManager.Vision;
    }

    public void EnterState()
    {
        stateManager.DangerOverlay.SetState(DangerState.Safe);
        stateManager.Vision.SetWatchingMode(WatchingMode.LostPlayer);
        stateManager.Watch(stateManager.Player.position);
        stateManager.Movement.StopAgent();
        stateManager.UIAnimation.PlayPupilExpand(vision.AttackAfterSeconds, true);
        stateManager.ToggleWitchLocator(true);

        // Start the frantic search
        if (searchRoutine != null)
            StopCoroutine(searchRoutine);

        searchRoutine = StartCoroutine(LookInCircles());
    }

    public void ExitState()
    {
        watchingTimer = 0;
        if (searchRoutine != null)
            StopCoroutine(searchRoutine);
        stateManager.ToggleWitchLocator(false);
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
            stateManager.TransitionToState(AIStates.PanicSearch);
        }

        // Found the player again
        if (stateManager.HasFoundPlayer())
            stateManager.TransitionToState(AIStates.Chase);
    }

    IEnumerator LookInCircles()
    {
        while (true)
        {
            // Random point around the player
            Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
            Vector3 lookPoint = stateManager.Player.position + randomDirection;

            // Use X Y for the search area
            lookPoint.y = stateManager.Player.position.y;

            stateManager.Watch(lookPoint);
            yield return new WaitForSeconds(lookDuration);
        }
    }
}
