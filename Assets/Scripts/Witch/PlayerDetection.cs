using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    // Collects the watchpoints that are watchable
    // Only the ones in the radius count
    public float WatchPointRange = 10f;
    public float WalkPointRange = 50f;
    private Transform Source;
    private AIStateManager stateManager;

    private void Start()
    {
        stateManager = GetComponent<AIStateManager>();
        Source = GameObject.FindWithTag("Player").transform;
    }

    public List<Transform> GetVisiblePoints()
    {
        List<Transform> inRangeWatchpoints = new List<Transform>();

        foreach (Transform watchpoint in stateManager.WatchPoints)
        {
            if (Vector3.Distance(Source.position, watchpoint.position) <= WatchPointRange)
                inRangeWatchpoints.Add(watchpoint);
        }

        return inRangeWatchpoints;
    }

    public List<Transform> GetWalkablePoints()
    {
        List<Transform> inRangeWalkPoints = new List<Transform>();

        foreach (Transform watchpoint in stateManager.Movement.walkPoints)
        {
            if (Vector3.Distance(Source.position, watchpoint.position) <= WalkPointRange)
                inRangeWalkPoints.Add(watchpoint);
        }

        return inRangeWalkPoints;
    }


    private void OnDrawGizmos()
    {
        if (stateManager == null)
            return;

        // Draw a simple circle around the entity to show detection range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Source.position, WatchPointRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Source.position, WalkPointRange);

        // Draw lines towards the watchpoints
        foreach (Transform watchpoint in GetVisiblePoints())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Source.position, watchpoint.position);
        }


        // Draw lines towards the watchpoints
        foreach (Transform walkpoint in GetWalkablePoints())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Source.position, walkpoint.position);
        }
    }
}
