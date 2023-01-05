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

    bool PlayerInVision = false;    // Currently in vision
    bool visible = false;
    private void Start()
    {
        currentWatchPoint = StandardWatchpoint.transform;
        witchMovement = GetComponent<WitchMovement>();
        StartCoroutine(CallingIntervall(WatchingIntervall));

        smoothingPosition = WatchPoints[0].position;
        currentTarget = WatchPoints[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        visible = PlayerVisible();
    }

    private void Update()
    {
        if (visible)
        {
            Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.red);
            alertTimer += Time.deltaTime;
            if (alertTimer >= AlertTime)
            {
                PlayerInVision = true;
                Slider.value += Time.deltaTime;
                witchMovement.GoToPlayerPoint();
                witchMovement.isLockedOnPlayer = true;
                // Focus on player!
                currentTarget = Player;
            }
        }
        else
        {
            // Witch lost Player after seeing him vision.
            if (PlayerInVision)
            {
                alertTimer = 0f;
                // Find the closest walkpoint and go to that
                witchMovement.isLockedOnPlayer = false;
                witchMovement.GoToNextPoint();
            }

            PlayerInVision = false;
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

    public void WatchNextPoint()
    {
        if (PlayerInVision)
            return;

        // No Watchpoints means no watching!
        if (WatchPoints.Length == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return;
        }

        // Find a random point of the Watchpoints
        watchIndex = Random.Range(0, WatchPoints.Length);

        // Store previous, set the new one
        oldWatchPoint = currentWatchPoint;
        currentWatchPoint = WatchPoints[watchIndex];
    }

    private void WatchSpot()
    {
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentTarget.position, ref SmoothVelocity, SmoothTime);
        ViewCone.transform.LookAt(smoothingPosition);
    }

    IEnumerator CallingIntervall(float timeIntervall)
    {
        while (true)
        {
            yield return new WaitUntil(() => visible == false);
            yield return new WaitForSeconds(timeIntervall);
            WatchNextPoint();
        }
    }
}
