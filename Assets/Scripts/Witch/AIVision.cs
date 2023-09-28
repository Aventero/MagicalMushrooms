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

    public GameObject ViewCone;
    public GameObject CurrentWatchTarget;
    public GameObject RaycastPoint;

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
    public float ChaseSmoothTime = 1.0f;
    public float LostPlayerSmoothTime = 2.0f;
    public float PanicSearchSmoothTime = 0.5f;
    public float SpottetPlayerSmoothTime = 0.5f;
    private float currentSmoothTime = 0f;


    private AIStateManager aiStateManager;
    [ReadOnly]
    public GameObject ObjectPlayerIsHidingBehind = null;

    // Only Look at Prop and Player!
    LayerMask layerMask;


    void Start()
    {
        layerMask = LayerMask.GetMask("Prop");
        layerMask |= LayerMask.GetMask("Player");
        layerMask |= LayerMask.GetMask("Interactable");
        aiStateManager = GetComponent<AIStateManager>();
        Player = aiStateManager.Player;
        Watch(aiStateManager.WatchPoints[0].position);
        SetWatchingMode(WatchingMode.Relaxed);
        smoothingPosition = currentWatchTarget;
    }

    void Update()
    {
        PlayerIsVisible = PlayerVisible();
    }

    public void WatchSpot()
    {
        Debug.DrawLine(ViewCone.transform.position, currentWatchTarget, new Color(0.1f, 0.1f, 0.1f));
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentWatchTarget, ref SmoothVelocity, currentSmoothTime);
        Vector3 relativeSmoothingPosition = smoothingPosition - ViewCone.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativeSmoothingPosition, ViewCone.transform.up);
        ViewCone.transform.rotation = rotation;
        CurrentWatchTarget.transform.position = smoothingPosition;

        //RotateAgent(currentWatchTarget);
    }


    // Use this for rotations????? Whenever looking at a new pooint?
    private void RotateAgent(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Do not rotate upwards or downwards
        directionToTarget.y = 0;

        if (directionToTarget == Vector3.zero)
        {
            return; // Avoid trying to look in a zero direction
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);

        // Set the agent's desired velocity so it moves in the direction we set.
        aiStateManager.Movement.agent.velocity = transform.forward * aiStateManager.Movement.agent.speed;
    }

    public void SetWatchingMode(WatchingMode watchSpeed)
    {
        currentSmoothTime = watchSpeed switch
        {
            WatchingMode.Relaxed => RelaxedSmoothTime,
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

    private bool PlayerVisible()
    {
        if (!StateManager.Instance.IsVisionConeOnPlayer)
        {
            ObjectPlayerIsHidingBehind = null;
            return false;
        }

        if (Physics.Linecast(ViewCone.transform.position, RaycastPoint.transform.position, out RaycastHit hitInfo, layerMask))
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
    SpottedPlayer,
    Chasing,
    LostPlayer,
    Paniced
}
