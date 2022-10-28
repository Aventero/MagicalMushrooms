using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WitchMovement : MonoBehaviour
{
    public Transform Destination;
    public Transform[] walkPoints;
    public float stoppingDistance = 1.0f;
    public float lookingRadius = 5.0f;

    private NavMeshAgent agent;
    private int walkIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        GoToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(agent.transform.position, Destination.position);
        if (distanceToPlayer < lookingRadius)
        {
            Vector3 stoppingPoint = (Destination.position - agent.transform.position).normalized * stoppingDistance;
            agent.destination = Destination.position - stoppingPoint;
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }

        // If point has be reached, set the next point
        Debug.DrawLine(agent.transform.position, agent.destination);
        Debug.DrawLine(agent.transform.position, walkPoints[walkIndex].position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawWireSphere(this.transform.position, lookingRadius);
    }

    public void GoToNextPoint()
    {
        // No Walkpoints means no walking!
        if (walkPoints.Length == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return;
        }

        // Agent walks to next destination
        agent.destination = walkPoints[walkIndex].position;

        // Choose the next
        walkIndex = (walkIndex + 1) % walkPoints.Length;
    }


}
