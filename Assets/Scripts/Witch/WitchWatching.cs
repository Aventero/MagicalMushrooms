using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WitchWatching : MonoBehaviour
{
    public Transform[] WatchPoints;
    private int watchIndex = 0;

    private Vector3 smoothingPosition;
    private Transform currentTarget;
    private Transform oldWatchPoint;
    private Transform currentWatchPoint;
    private Vector3 SmoothVelocity = Vector3.zero;

    public float AlertTime = 0.1f;
    private float alertTimer = 0f;
    public float SmoothTime = 0.3f;
    public float WatchingIntervall = 1f;
    public Transform Player;
    public GameObject ViewCone;
    public GameObject StandardWatchpoint;
    public Slider Slider;
    private WitchMovement witchMovement;
    bool PlayerActuallyVision = false;    // Currently in vision
    bool PlayerIsVisible = false;
    public UnityAction IsDoneWatching;
    
    private void Start()
    {
        currentWatchPoint = StandardWatchpoint.transform;
        witchMovement = GetComponent<WitchMovement>();
        witchMovement.HasReachedDestination += LookAround;
        witchMovement.StartsMovingAgain += ChooseNearestWatchPoint;

        smoothingPosition = WatchPoints[0].position;
        currentTarget = WatchPoints[0];
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerIsVisible = PlayerVisible();
    }

    private void Update()
    {
        if (PlayerIsVisible)
        {
            Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.red);
            alertTimer += Time.deltaTime;
            if (alertTimer >= AlertTime)
            {
                PlayerActuallyVision = true;
                Slider.value += Time.deltaTime;
                witchMovement.GoToPlayer();
                witchMovement.isLockedOnPlayer = true;

                // Focus on player!
                currentTarget = Player;
            }
        }
        else
        {
            // Witch lost Player after seeing him vision.
            if (PlayerActuallyVision)
            {
                alertTimer = 0f;
                // Find the closest walkpoint and go to that
                witchMovement.isLockedOnPlayer = false;
                witchMovement.ChooseNextWalkPoint();
                GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
            }

            PlayerActuallyVision = false;
            Slider.value -= Time.deltaTime;

            // Player escaped, watch points
            currentTarget = currentWatchPoint;
            Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.blue);
            Debug.DrawLine(ViewCone.transform.position, currentTarget.position, Color.white);
        }

        WatchSpot();
    }

    private bool PlayerVisible()
    {
        if (!StateManager.Instance.WitchConeOnPlayer)
            return false;

        if (Physics.Linecast(ViewCone.transform.position, Player.transform.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void ChooseNearestWatchPoint()
    {
        if (PlayerActuallyVision)
            return;

        // No Watchpoints means no watching!
        if (WatchPoints.Length == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return;
        }

        // Choose one randomly
        // Find the closest walkpoint
        // Walk to the point while looking at the point?
        oldWatchPoint = currentWatchPoint;
        currentWatchPoint = NearestWatchpoint(witchMovement.currentWalkPoint.position, WatchPoints);
    }

    private void LookAround()
    {
        StartCoroutine(LookAroundRandomly(UnityEngine.Random.Range(1, 3), UnityEngine.Random.Range(1, 3)));
    }

    IEnumerator LookAroundRandomly(int times, float waitTimeInBetween)
    {
        while (times >= 0)
        {
            times--;

            // Find a random point of the Watchpoints
            watchIndex = UnityEngine.Random.Range(0, WatchPoints.Length);

            // Store previous, set the new one
            oldWatchPoint = currentWatchPoint;
            currentWatchPoint = WatchPoints[watchIndex];

            yield return new WaitForSeconds(waitTimeInBetween);
        }
        IsDoneWatching.Invoke();
    }

    public Transform NearestWatchpoint(Vector3 position, Transform[] points)
    {
        // Get the shortest path with, wich is not the current & previous one!
        if (points.Length == 0)
        {
            Debug.LogError("Points array is empty!");
            return null;
        }

        Transform shortestPoint = points[0];
        foreach (Transform point in points)
        {
            if (Vector3.Distance(position, point.position) < Vector3.Distance(position, shortestPoint.position))
                shortestPoint = point;
        }
        return shortestPoint;
    }

    private void WatchSpot()
    {
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentTarget.position, ref SmoothVelocity, SmoothTime);
        ViewCone.transform.LookAt(smoothingPosition);
    }
}
