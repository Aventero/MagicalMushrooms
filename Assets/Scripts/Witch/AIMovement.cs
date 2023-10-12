using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }
    public Vector3 currentWalkPoint;

    // Animation
    public Animator animator { get; private set; }
    public float AddedStopThreshold = 0.1f;
    private AIStateManager stateManager;

    void Awake()
    {
        stateManager = GetComponent<AIStateManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        animator = GetComponent<Animator>();
        currentWalkPoint = transform.position;
    }

    public void SetWalkPoint(Vector3 point)
    {
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

    public void MoveToNextPoint()
    {
        // Choose a random point of interest
        List<Transform> points = stateManager.PlayerDetection.GetViewPointsAroundPlayerAndSome();

        if (points.Count <= 0)
            return;

        int index = Random.Range(0, points.Count);
        agent.destination = points[index].position;
    }

    public void AnimateWitch()
    {
        if (agent.velocity.sqrMagnitude <= AddedStopThreshold + agent.stoppingDistance)
            animator.SetBool("Stay", true);
        else
            animator.SetBool("Stay", false);
    }
}

