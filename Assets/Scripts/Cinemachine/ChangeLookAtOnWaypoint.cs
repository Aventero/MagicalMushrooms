using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class ChangeLookAtOnWaypoint : MonoBehaviour
{
    [System.Serializable]
    public class WaypointLookAtPair
    {
        public float waypointPosition;  // Position on the dolly track (0 to 1)
        public Transform lookAtTarget;  // Target to look at when reaching this waypoint
        public bool stopAtThisWaypoint; // Should the cart stop at this waypoint?
        public float stopDuration;      // Duration for which the cart should stop
        public UnityEvent onWaypointReached; 
    }

    public CinemachineVirtualCamera vCam;
    public CinemachineDollyCart dollyCart;
    public List<WaypointLookAtPair> waypointLookAtPairs = new List<WaypointLookAtPair>();
    public float dollySpeed = 5f; // Set the default speed of the dolly cart here

    private int currentIndex = 0;

    private void Start()
    {
        if (dollyCart)
            dollyCart.m_Speed = dollySpeed;
    }

    private void Update()
    {
        if (currentIndex < waypointLookAtPairs.Count)
        {
            // Check if the dolly cart has passed the current waypoint
            if (dollyCart.m_Position >= waypointLookAtPairs[currentIndex].waypointPosition)
            {
                // Set the LookAt target for the vCam
                vCam.LookAt = waypointLookAtPairs[currentIndex].lookAtTarget;

                waypointLookAtPairs[currentIndex].onWaypointReached?.Invoke();

                // Check if we should stop at this waypoint
                if (waypointLookAtPairs[currentIndex].stopAtThisWaypoint)
                {
                    dollyCart.m_Speed = 0f; // Stop the dolly cart
                    StartCoroutine(ResumeAfterDelay(waypointLookAtPairs[currentIndex].stopDuration)); // Resume motion after specified delay

                }

                // Move on to the next waypoint
                currentIndex++;
            }
        }
    }

    IEnumerator ResumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dollyCart.m_Speed = dollySpeed; // Resume speed after delay
    }
}
