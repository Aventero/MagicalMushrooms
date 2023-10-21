using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using UnityEditor.TerrainTools;

public class ChangeLookAtOnWaypoint : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public float position;          // Position on the dolly track (0 to 1)
        public float stopDuration;      // Duration for which the cart should stop
        public Transform lookAtTarget;  // Target to look at when reaching this waypoint
        public UnityEvent onWaypointReached; 
    }

    public CinemachineVirtualCamera vCam;
    public CinemachineDollyCart dollyCart;
    public List<Waypoint> waypoints = new List<Waypoint>();
    public float dollySpeed = 5f; // Set the default speed of the dolly cart here

    private int index = 0;

    private void Start()
    {
        StateManager.Instance.EndedDialogEvent.AddListener(ResumeMovement);
        StateManager.Instance.StartedDialogEvent.AddListener(PauseMovement);

        if (dollyCart)
            dollyCart.m_Speed = dollySpeed;
    }

    private void Update()
    {
        if (index >= waypoints.Count)
            return;

        // Check if the dolly cart has passed the current waypoint
        if (HasPassedWaypoint())
        {
            if (waypoints[index].lookAtTarget != null)
                vCam.LookAt = waypoints[index].lookAtTarget;

            // Waypoint contains a Reached Method
            waypoints[index].onWaypointReached?.Invoke();

            // Check if we should stop at this waypoint
            if (waypoints[index].stopDuration >= 0.001f && waypoints[index].onWaypointReached.GetPersistentEventCount() <= 0)
            {
                PauseMovement();
                StartCoroutine(ResumeAfterDelay(waypoints[index].stopDuration)); // Resume motion after specified delay
            }

            // Move on to the next waypoint
            index++;
        }
    }

    private void PauseMovement()
    {
        dollyCart.m_Speed = 0f; // Stop the dolly cart
    }

    private void ResumeMovement()
    {
        dollyCart.m_Speed = dollySpeed;
    }

    private bool HasPassedWaypoint()
    {
        return dollyCart.m_Position >= waypoints[index].position;
    }


    IEnumerator ResumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dollyCart.m_Speed = dollySpeed; // Resume speed after delay
    }
}
