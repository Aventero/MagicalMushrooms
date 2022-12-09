using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WitchWatching : MonoBehaviour
{
    public Transform[] WatchPoints;
    private int watchIndex = 0;
    private bool isWatching = false;
    private Transform currentWatchPoint;
    private Transform previousWatchPoint;

    public Transform Player;
    public GameObject ViewCone;
    public GameObject StandardWatchpoint;
    public Slider Slider;

    private void Start()
    {
        currentWatchPoint = StandardWatchpoint.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWatching)
            WatchNextPoint();
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        bool PlayerInVIsion = false;
        if (StateManager.Instance.WitchConeOnPlayer)
        {
            // Is in vision, but is the player behind an object?
            RaycastHit hit;
            if (Physics.Raycast(ViewCone.transform.position, Player.transform.position - ViewCone.transform.position, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Player")
                {
                    PlayerInVIsion = true;
                    Slider.value += Time.deltaTime;
                    Debug.DrawLine(ViewCone.transform.position, 100f * (Player.transform.position - ViewCone.transform.position), Color.magenta);
                }
            }
        }

        if (!PlayerInVIsion)
        {
            Slider.value -= Time.deltaTime;
        }
    }



    public void WatchNextPoint()
    {
        // No Watchpoints means no watching!
        if (WatchPoints.Length == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return;
        }

        // Witch is Watching something -> Lock the Coroutine
        isWatching = true;

        // Find a random point of the Watchpoints
        watchIndex = Random.Range(0, WatchPoints.Length);

        // Store previous, set the new one
        previousWatchPoint = currentWatchPoint;
        currentWatchPoint = WatchPoints[watchIndex];
        if (Random.Range(0f, 1f) <= 0.2f)
            currentWatchPoint = StandardWatchpoint.transform;

        StartCoroutine(OrbitWatchPoint(2f));
    }

    IEnumerator OrbitWatchPoint(float time)
    {
        // Lerp the Cone Towards the target
        float elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;

            // ANIMATION CURVE?
            Vector3 intermediate = Vector3.Lerp(previousWatchPoint.position, currentWatchPoint.position, elapsed / time); 
           // Debug.DrawLine(ViewCone.transform.position, intermediate, Color.white);
            ViewCone.transform.LookAt(intermediate);
            yield return null;  
        }

        // Lock the Watchpoint for some more time
        elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            //Debug.DrawLine(ViewCone.transform.position, currentWatchPoint.position, Color.white);
            ViewCone.transform.LookAt(currentWatchPoint.position);
            yield return null;
        }

        // Set it free!
        isWatching = false;
    }
}
