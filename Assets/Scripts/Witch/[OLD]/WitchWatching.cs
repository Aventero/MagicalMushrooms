using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WitchWatching : MonoBehaviour
{
    public GameObject WatchPointsParent;
    private List<Transform> WatchPoints;

    private Vector3 smoothingPosition;
    private Transform currentWatchTarget;
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
    bool IsHuntingPlayer = false;
    bool IsWatchingLastSeenSpot = false;
    bool PlayerIsVisible = false;
    public UnityAction IsDoneWatching;
    public UnityAction HuntingPlayer;
    
    
    private void Start()
    {
        currentSmoothTime = SmoothTime;
        currentWatchTarget = StandardWatchpoint.transform;
        witchMovement = GetComponent<WitchMovement>();
        witchMovement.ReadyToLookAround += LookAround;
        witchMovement.StartsMovingAgain += ChooseNearestWatchPoint;

        WatchPoints = new List<Transform>(WatchPointsParent.GetComponentsInChildren<Transform>().Where(Point => Point != WatchPointsParent.transform));

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
            ReturnToNormalMovement();
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
                Slider.value = Slider.maxValue;
                
                IsHuntingPlayer = true;
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
                ReturnToNormalMovement();
                IsHuntingPlayer = false;
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

    private void ReturnToNormalMovement()
    {
        Slider.value = Slider.minValue;
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;

        currentSmoothTime = SmoothTime;
        alertTimer = 0f;
        losingTimer = 0f;

        // Find the closest walkpoint and go to that
        witchMovement.isLockedOnPlayer = false;
        IsWatchingLastSeenSpot = false;
        witchMovement.ChooseNextWalkPointImmediately();
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


    // Called when the witch starts moving again! -> The next Walkpoint has already been set
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
        Vector3 forwardToWalkpoint = witchMovement.currentWalkPoint.position - transform.position;
        List<Transform> visiblePointsAtNextDestination = CalculateVisiblePoints(witchMovement.currentWalkPoint.position, forwardToWalkpoint, 75f);
        if (visiblePointsAtNextDestination.Count == 0)
        {
            currentWatchTarget = StandardWatchpoint.transform;
        }
        else
            currentWatchTarget = visiblePointsAtNextDestination[UnityEngine.Random.Range(0, visiblePointsAtNextDestination.Count)];

        if (EasyAngle(transform.position, transform.forward, currentWatchTarget.position) > 75f)
        {
            // the new point is behind the witch, look at the standard and then at the corrent one
            Transform tmp = currentWatchTarget;
            currentWatchTarget = StandardWatchpoint.transform;
            yield return new WaitUntil(() => EasyAngle(transform.position, transform.forward, witchMovement.currentWalkPoint.position) < 45f);
            currentWatchTarget = tmp;
        }

        yield return null;
    }

    private List<Transform> CalculateVisiblePoints(Vector3 desiredPoint, Vector3 forward, float viewAngle)
    {
        List<Transform> visibleWatchPoints = new List<Transform>();
        //Vector2 forward2D = new Vector2(forward.x, forward.z);
        foreach (Transform watchPoint in WatchPoints)
        {
           // Vector2 witchTowardsPoint = new Vector2(watchPoint.position.x, watchPoint.position.z) - new Vector2(desiredPoint.x, desiredPoint.z);
            float angle = EasyAngle(desiredPoint, forward, watchPoint.position);
           // float angleToPoint = Vector2.Angle(forward2D, witchTowardsPoint);
            if (angle <= viewAngle)
            {
                visibleWatchPoints.Add(watchPoint);
                Debug.DrawLine(transform.position, watchPoint.position, Color.magenta, 2f);
            }
        }

        return visibleWatchPoints;
    }

    public static float EasyAngle(Vector3 position, Vector3 forward, Vector3 desiredPoint)
    {
        Vector2 forward2D = new Vector2(forward.x, forward.z);
        Vector2 towardsPoint = new Vector2(desiredPoint.x, desiredPoint.z) - new Vector2(position.x, position.z);
        return Vector2.Angle(forward2D, towardsPoint);
    }

    private void LookAround()
    {
        List<Transform> visiblePoints = CalculateVisiblePoints(transform.position, transform.forward, 75f);
        StartCoroutine(LookAroundRandomly(WatchWaitTime, visiblePoints));
    }

    IEnumerator LookAroundRandomly(float waitTimeInBetween, List<Transform> visiblePoints)
    {
        yield return new WaitUntil(() => !IsWatchingLastSeenSpot || !IsHuntingPlayer );

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

    private Transform CalcualteNearestWatchpoint(Vector3 position, List<Transform> points)
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
        Vector3 relativeSmoothingPosition = smoothingPosition - ViewCone.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativeSmoothingPosition, ViewCone.transform.up);
        ViewCone.transform.rotation = rotation;
    }
}
