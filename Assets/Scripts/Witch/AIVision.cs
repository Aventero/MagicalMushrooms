using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AIVision : MonoBehaviour
{
    public GameObject ViewCone;
    public GameObject CurrentWatchTarget;
    public GameObject RaycastPoint;

    // Player watching
    private Transform Player;
    private bool PlayerIsVisible = false;
    public float AlertTime = 0.5f;
    private float findingTimer = 0f;
    public float LosingTime = 1.0f;
    private float losingTimer = 0f;

    // Point Watching
    public Vector3 currentWatchTarget { get; set; }
    private Vector3 SmoothVelocity = Vector3.zero;
    private Vector3 smoothingPosition;
    public float SmoothTime = 0.5f;
    public float HuntSmoothTime = 0.5f;
    private float currentSmoothTime = 0f;

    private AIStateManager aiStateManager;
    [ReadOnly]
    public GameObject ObjectPlayerIsHidingBehind = null;

    void Start()
    {
        aiStateManager = GetComponent<AIStateManager>();
        Player = aiStateManager.Player;
        Watch(aiStateManager.WatchPoints[0].position);
        RelaxedWatching();
        smoothingPosition = currentWatchTarget;
    }

    void FixedUpdate()
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
    }

    public void PlayerWatching()
    {
        currentSmoothTime = HuntSmoothTime;
    }

    public void RelaxedWatching()
    {
        currentSmoothTime = SmoothTime;
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

        // Only Look at Prop and Player!
        LayerMask layerMask = LayerMask.GetMask("Prop");
        layerMask |= LayerMask.GetMask("Player");
        layerMask |= LayerMask.GetMask("Interactable");

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
            if (findingTimer >= AlertTime)
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
