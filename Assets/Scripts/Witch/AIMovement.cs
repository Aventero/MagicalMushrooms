using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }
    public Vector3 currentWalkPoint;
    private Vector3 previousWalkPoint;
    public List<Transform> walkPoints { get; private set; }
    public GameObject WalkPointsParent;

    // Animation
    public Animator animator { get; private set; }
    public float AddedStopThreshold = 0.1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        animator = GetComponent<Animator>();

        walkPoints = new List<Transform>(WalkPointsParent.GetComponentsInChildren<Transform>().Where(point => point != WalkPointsParent.transform));
        currentWalkPoint = previousWalkPoint = walkPoints[0].position;
    }

    public void SetWalkPoint(Vector3 point)
    {
        previousWalkPoint = currentWalkPoint;
        currentWalkPoint = point;
        agent.destination = currentWalkPoint;
    }

    public void StartAgent()
    {
        agent.isStopped = false;
    }

    public void StopAgent()
    {
        agent.isStopped = true;
        agent.destination = agent.transform.position;
    }

    public Transform FindNewWalkpoint()
    {
        // No Walkpoints means no walking!
        if (walkPoints.Count == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return null;
        }

        // Get the shortest path, wich is not the current & previous one!
        List<Transform> closestWalkPoints = new List<Transform>();
        foreach (Transform walkPoint in walkPoints)
        {
            if (walkPoint.position != currentWalkPoint && walkPoint.position != previousWalkPoint)
            {
                closestWalkPoints.Add(walkPoint);
            }
        }
        closestWalkPoints.Sort((p1, p2) => Vector3.Distance(transform.position, p1.position).CompareTo(Vector3.Distance(transform.position, p2.position)));

        // Randomly choose one of the 5 closest points
        Transform shortestPoint = closestWalkPoints[Random.Range(0, closestWalkPoints.Count / 2)];

        //for (int i = 0; i < closestWalkPoints.Count / 2; i++)

        Debug.DrawLine(transform.position, shortestPoint.position, Color.yellow, 1f);
        return shortestPoint;
    }

    public void AnimateWitch()
    {
        if (agent.velocity.sqrMagnitude <= AddedStopThreshold + agent.stoppingDistance)
            animator.SetBool("Stay", true);
        else
            animator.SetBool("Stay", false);
    }
}