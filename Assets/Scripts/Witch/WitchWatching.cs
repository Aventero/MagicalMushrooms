using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WitchWatching : MonoBehaviour
{
    public GameObject WatchPointsParent;
    private List<Transform> WatchPoints;
    private int watchIndex = 0;

    private Vector3 smoothingPosition;
    private Transform currentWatchTarget;
    //private Transform oldWatchPoint;
    private Vector3 SmoothVelocity = Vector3.zero;

    public float AlertTime = 0.5f;
    private float alertTimer = 0f;
    public float LosingTime = 1.0f;
    private float losingTimer = 0f;
    public float SmoothTime = 0.3f;
    private float currentSmoothTime;
    public float WatchWaitTime = 2f;
    public Transform Player;
    public GameObject ViewCone;
    public GameObject StandardWatchpoint;
    public Slider Slider;
    private WitchMovement witchMovement;
    bool IsHuntingPlayer = false;    // Currently in vision
    bool IsWatchingLastSeenSpot = false;
    bool PlayerIsVisible = false;
    public UnityAction IsDoneWatching;
    
    
    private void Start()
    {
        currentSmoothTime = SmoothTime;
        currentWatchTarget = StandardWatchpoint.transform;
        witchMovement = GetComponent<WitchMovement>();
        witchMovement.ReadyToLookAround += LookAround;
        witchMovement.StartsMovingAgain += ChooseNearestWatchPoint;

        List<Transform> childWatchPoints = new List<Transform>(WatchPointsParent.GetComponentsInChildren<Transform>());
        childWatchPoints.Remove(WatchPointsParent.transform);
        WatchPoints = childWatchPoints;

        smoothingPosition = WatchPoints[0].position;
        currentWatchTarget = WatchPoints[0];
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerIsVisible = PlayerVisible();
    }

    private void Update()
    {
        if (HasJustFoundPlayer())
            HuntPlayer();

        if (HasJustLostPlayer())
        {
            IsWatchingLastSeenSpot = true;
            StartCoroutine(ReturnToNormalMovement(WatchWaitTime));
        }

        if (IsHuntingPlayer)
            HuntPlayer();

        Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.blue);
        Debug.DrawLine(ViewCone.transform.position, currentWatchTarget.position, Color.white);

        WatchSpot();
    }

    private bool HasJustFoundPlayer()
    {
        if (PlayerIsVisible && !IsHuntingPlayer)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer >= AlertTime)
            {
                StopAllCoroutines();
                IsHuntingPlayer = true;
                Debug.Log("Found!");
                return true;
            }
        }

        return false;
    }

    private bool HasJustLostPlayer()
    {
        if (!PlayerIsVisible && IsHuntingPlayer)
        {
            losingTimer += Time.deltaTime;
            if (losingTimer >= LosingTime)
            {
                IsHuntingPlayer = false;
                Debug.Log("Lost player!");
                return true;
            }
        }

        return false;
    }

    private void HuntPlayer()
    {
        witchMovement.isLockedOnPlayer = true;
        witchMovement.GoToPlayer();
        currentSmoothTime = 0.01f;

        // Focus on player!
        currentWatchTarget = Player;
        Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.red);
    }

    IEnumerator ReturnToNormalMovement(float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;

        currentSmoothTime = SmoothTime;
        alertTimer = 0f;
        losingTimer = 0f;
        //currentWatchTarget = currentWatchPoint;

        // Find the closest walkpoint and go to that
        witchMovement.isLockedOnPlayer = false;
        IsWatchingLastSeenSpot = false;
        witchMovement.ChooseNextWalkPointImmediately();
        yield return new WaitForEndOfFrame();
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
        StartCoroutine(ChooseNearestWatchPointCoroutine());
    }

    IEnumerator ChooseNearestWatchPointCoroutine()
    {
        yield return new WaitUntil(() => !IsWatchingLastSeenSpot || !IsHuntingPlayer);

        // No Watchpoints means no watching!
        if (WatchPoints.Count == 0)
        {
            Debug.Log("No walkingpoints set up!");
            yield break;
        }

        // Choose one randomly
        currentWatchTarget = NearestWatchpoint(witchMovement.currentWalkPoint.position, WatchPoints);
        yield return null;
    }

    private List<Transform> CalculateVisiblePoints()
    {
        List<Transform> visibleWatchPoints = new List<Transform>();
        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        foreach (Transform point in WatchPoints)
        {
            Vector2 witchTowardsPoint = new Vector2(point.position.x, point.position.z) - new Vector2(transform.position.x, transform.position.z);
            float angleToPoint = Vector2.Angle(forward, witchTowardsPoint);
            if (angleToPoint <= 90f)
            {
                visibleWatchPoints.Add(point);
                Debug.DrawLine(transform.position, point.position, Color.magenta, 2f);
            }
        }

        return visibleWatchPoints;
    }

    private void LookAround()
    {
        List<Transform> visiblePoints = CalculateVisiblePoints();
        StartCoroutine(LookAroundRandomly(WatchWaitTime, visiblePoints));
    }

    IEnumerator LookAroundRandomly(float waitTimeInBetween, List<Transform> visiblePoints)
    {
        yield return new WaitUntil(() => !IsWatchingLastSeenSpot || !IsHuntingPlayer );

        // relax
        currentWatchTarget = StandardWatchpoint.transform;
        yield return new WaitForSeconds(waitTimeInBetween / 2f);

        int times = 0;
        foreach (Transform point in visiblePoints)
        {
            if (times >= 3)
            {
                IsDoneWatching.Invoke();
                yield break; // Return the Coroutine
            }

            // Store previous, set the new one
            currentWatchTarget = point;
            times++;

            yield return new WaitForSeconds(waitTimeInBetween);
        }

        IsDoneWatching.Invoke();
    }

    private Transform NearestWatchpoint(Vector3 position, List<Transform> points)
    {
        // Get the shortest path with, wich is not the current & previous one!
        if (points.Count == 0)
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
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentWatchTarget.position, ref SmoothVelocity, currentSmoothTime);
        ViewCone.transform.LookAt(smoothingPosition);
    }
}
