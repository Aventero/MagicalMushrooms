using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AIVision : MonoBehaviour
{
    [Header("Player Detection")]
    public float PerceptionLag = 0.5f;
    private float findingTimer = 0f;
    public float LosingTime = 1.0f;
    private float losingTimer = 0f;

    [Header("View Cone")]
    public float DistanceScaling = 1f;
    public float ScalingStrength = 10f;
    public float MaxScaling = 10f;
    public GameObject ViewCone;
    public GameObject HeadWatchTarget;
    public GameObject RaycastPoint;
    private Vector3 originalVisionScale;
    [ReadOnly]
    public float distanceToWatchPoint;

    // Player watching
    private Transform Player;
    private bool PlayerIsVisible = false;

    [Header("Chase to Attack")]
    public float AttackAfterSeconds = 5f;
    public float ChaseTime { get; set; }

    public Vector3 currentWatchTarget { get; set; }
    private Vector3 SmoothVelocity = Vector3.zero;
    private Vector3 smoothingPosition;

    [Header("Vision Smoothing")]
    public float RelaxedSmoothTime = 1.5f;
    public float PatrolSmoothTime = 0.25f;
    public float ChaseSmoothTime = 1.0f;
    public float LostPlayerSmoothTime = 2.0f;
    public float PanicSearchSmoothTime = 0.5f;
    public float SpottetPlayerSmoothTime = 0.5f;
    private float currentSmoothTime = 0f;

    private AIStateManager stateManager;
    [ReadOnly]
    public GameObject ObjectPlayerIsHidingBehind = null;
    LayerMask lookAtMask;
    private Vector3 targetScale;
    void Start()
    {
        lookAtMask = LayerMask.GetMask("Prop");
        lookAtMask |= LayerMask.GetMask("Player");
        lookAtMask |= LayerMask.GetMask("Interactable");
        originalVisionScale = ViewCone.transform.localScale;
        stateManager = GetComponent<AIStateManager>();
        Player = stateManager.Player;
        Watch(stateManager.WatchPoints[0].position);
        SetWatchingMode(WatchingMode.Relaxed);
        smoothingPosition = currentWatchTarget;
    }

    void Update()
    {
        PlayerIsVisible = PlayerVisible();
    }

    public void WatchCurrentTarget()
    {
        TurnTowardsTarget();
        ScaleVisionByDistance();
    }

    public void TurnTowardsTarget()
    {
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentWatchTarget, ref SmoothVelocity, currentSmoothTime);
        Vector3 relativeSmoothingPosition = smoothingPosition - ViewCone.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativeSmoothingPosition, ViewCone.transform.up);
        ViewCone.transform.rotation = rotation;
        HeadWatchTarget.transform.position = smoothingPosition;
        Debug.DrawLine(stateManager.Vision.ViewCone.transform.position, smoothingPosition, Color.white);
        Debug.DrawLine(stateManager.Vision.ViewCone.transform.position, currentWatchTarget, Color.yellow);
    }

    public bool HasReachedTarget()
    {
        float threshold = 0.25f;
        return Vector3.Distance(smoothingPosition, currentWatchTarget) < threshold;
    }

    public void SetWatchingMode(WatchingMode watchSpeed)
    {
        currentSmoothTime = watchSpeed switch
        {
            WatchingMode.Relaxed => RelaxedSmoothTime,
            WatchingMode.Patrol => PatrolSmoothTime,
            WatchingMode.SpottedPlayer => SpottetPlayerSmoothTime,
            WatchingMode.Chasing => ChaseSmoothTime,
            WatchingMode.LostPlayer => LostPlayerSmoothTime,
            WatchingMode.Paniced => PanicSearchSmoothTime,
            _ => RelaxedSmoothTime,
        };
    }

    public void Watch(Vector3 point)
    {
        currentWatchTarget = point;
    }

    public void ScaleVisionByDistance()
    {
        float distanceToWatchpoint = Vector3.Distance(ViewCone.transform.position, currentWatchTarget);

        // Lerp factor based on distance
        float maxDistance = 25.0f;
        float lerpFactor = Mathf.Clamp01(distanceToWatchpoint / maxDistance);

        // Additional scale for the Z-axis.
        float maxAdditionalScale = 30.0f;
        float additionalZScale = Mathf.Lerp(maxAdditionalScale, 0, lerpFactor);

        targetScale = new Vector3(originalVisionScale.x, originalVisionScale.y, originalVisionScale.z - additionalZScale);

        // Interpolate towards the target scale.
        float lerpSpeed = 0.1f;
        ViewCone.transform.localScale = Vector3.Lerp(ViewCone.transform.localScale, targetScale, lerpSpeed);
    }


    public bool RotateAgent(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;
        directionToTarget = directionToTarget.normalized;

        // Do not rotate upwards or downwards

        if (directionToTarget == Vector3.zero)
            return false; 

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Only XZ plane
        Vector3 currentForwardOnXZ = transform.forward;
        currentForwardOnXZ.y = 0;

        // Done rotating?
        float dot = Vector3.Dot(currentForwardOnXZ.normalized, directionToTarget);
        if (dot > 0.99f)
            return true;
        return false;
    }



    private bool PlayerVisible()
    {
        if (!StateManager.Instance.IsVisionConeOnPlayer)
        {
            ObjectPlayerIsHidingBehind = null;
            return false;
        }

        if (Physics.Linecast(ViewCone.transform.position, RaycastPoint.transform.position, out RaycastHit hitInfo, lookAtMask))
        {
            Debug.DrawLine(ViewCone.transform.position, hitInfo.transform.position, Color.green);

            if (hitInfo.collider.CompareTag("Draggable"))
            {
                ObjectPlayerIsHidingBehind = hitInfo.transform.gameObject;
                return false;
            }

            if (hitInfo.transform.CompareTag("Player"))
            {
                Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.magenta);
                ObjectPlayerIsHidingBehind = null;
                return true;
            }
        }
        ObjectPlayerIsHidingBehind = null;
        return false;
    }

    public bool FoundPlayer()
    {
        if (PlayerIsVisible)
        {
            findingTimer += Time.deltaTime;
            if (findingTimer >= PerceptionLag)
            {
                losingTimer = 0f;
                return true;
            }

            return false;
        }

        return false;
    }

    public bool LostPlayer()
    {
        if (!PlayerIsVisible)
        {
            losingTimer += Time.deltaTime;
            if (losingTimer >= LosingTime)
            {
                findingTimer = 0f;
                return true;
            }

            return false;
        }

        return false;
    }
}

public enum WatchingMode
{
    Relaxed,
    Patrol,
    SpottedPlayer,
    Chasing,
    LostPlayer,
    Paniced
}
