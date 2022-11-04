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
    public float lookingAngle = 45.0f;   // Angle from the from the forward vector

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

        // Get the angle on the xz plane, from agent to player (from 0 to 180)
        float angleToPlayer = Vector2.Angle(new Vector2(agent.transform.forward.x, agent.transform.forward.z), new Vector2(Destination.position.x, Destination.position.z) - new Vector2(agent.transform.position.x, agent.transform.position.z)); 
        if (distanceToPlayer < lookingRadius && angleToPlayer <= lookingAngle)
        {
            Debug.DrawLine(agent.transform.position, Destination.position, Color.cyan);
            Vector3 stoppingPoint = (Destination.position - agent.transform.position).normalized * stoppingDistance;
            agent.destination = Destination.position - stoppingPoint;
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }

        // If point has be reached, set the next point
       // Debug.DrawLine(agent.transform.position, agent.destination);
       // Debug.DrawLine(agent.transform.position, walkPoints[walkIndex].position);
        float posAngle = Mathf.Deg2Rad * (agent.transform.localEulerAngles.y + lookingAngle);   // Angle in Radians
        float negAngle = Mathf.Deg2Rad * (agent.transform.localEulerAngles.y - lookingAngle);   // -Angle in Radians
        Debug.DrawLine(agent.transform.position, agent.transform.position + new Vector3(lookingRadius * Mathf.Sin(posAngle), agent.transform.position.y, lookingRadius * Mathf.Cos(posAngle)), Color.green);
        Debug.DrawLine(agent.transform.position, agent.transform.position + new Vector3(lookingRadius * Mathf.Sin(negAngle), agent.transform.position.y, lookingRadius * Mathf.Cos(negAngle)), Color.yellow);
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
