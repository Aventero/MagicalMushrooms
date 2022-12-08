using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WitchMovement : MonoBehaviour
{
    public Transform player;
    public Transform[] walkPoints;
    public float lookingDistance = 5.0f;
    public float lookingAngle = 45.0f;   // Angle from the forward vector
    public float alwaysFoundDistance = 10.0f;
    public float catchingDistance = 5.5f;
    public int PlayerDamage = 1;

    private NavMeshAgent agent;
    private int walkIndex = 0;
    private bool isLockedOnPlayer = false;
    public float secondsTillCaught = 3.0f;
    private float timeInsideCatchArea = 0.0f;

    Animator animator;
    //private bool pickingUpIsPlaying = false;
    public Transform witchCameraPosition;
    public Transform witchCameraTarget;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.autoBraking = true;
        GoToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        // Once the player was too long in the area, play the animation
        if (timeInsideCatchArea >= secondsTillCaught)
        {
            timeInsideCatchArea = 0;
            animator.SetTrigger("PickUp");
            this.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
            StateManager.Instance.isLockedOnWitchHead = true;
            StateManager.Instance.DealDamageEvent(PlayerDamage);
            return;
        }

        // Don't move, while pickingUp or in transition to another animation
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PickUpPlayer") || animator.IsInTransition(0))
        {
            player.transform.position = witchCameraPosition.position;
            Camera.main.transform.LookAt(witchCameraTarget, Vector3.up);
            return;
        }

        StateManager.Instance.isLockedOnWitchHead = false;

        // On XZ Plane
        float distance2DToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z));

        // Get the angle on the xz plane, from agent to player (from 0 to 180)
        float angleToPlayer = Vector2.Angle(new Vector2(agent.transform.forward.x, agent.transform.forward.z), new Vector2(player.position.x, player.position.z) - new Vector2(agent.transform.position.x, agent.transform.position.z));
        float posAngle = Mathf.Deg2Rad * (agent.transform.localEulerAngles.y + lookingAngle);   // Angle in Radians
        float negAngle = Mathf.Deg2Rad * (agent.transform.localEulerAngles.y - lookingAngle);   // -Angle in Radians
        Debug.DrawLine(agent.transform.position, agent.transform.position + new Vector3(lookingDistance * Mathf.Sin(posAngle), 0, lookingDistance * Mathf.Cos(posAngle)), Color.green);
        Debug.DrawLine(agent.transform.position, agent.transform.position + new Vector3(lookingDistance * Mathf.Sin(negAngle), 0, lookingDistance * Mathf.Cos(negAngle)), Color.yellow);

        // Raycast will only hit objects (Chair, Table, etc.), if it hits one and the player is behind it, don't move there
        bool playerIsBehindObject = Physics.Raycast(agent.transform.position, (player.position - agent.transform.position), LayerMask.NameToLayer("Ignore Raycast"));

        if ((distance2DToPlayer < lookingDistance && angleToPlayer <= lookingAngle && !playerIsBehindObject) || distance2DToPlayer <= alwaysFoundDistance)
        {
            Debug.DrawRay(agent.transform.position, (player.position - agent.transform.position), Color.cyan);
            agent.destination = player.position;
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
            isLockedOnPlayer = true;

            // Player is inside the catching area
            if (distance2DToPlayer <= catchingDistance)
            {
                timeInsideCatchArea += Time.deltaTime;
            }
            else
            {
                // Reset the catching time, cause the player is not inside it
                timeInsideCatchArea = 0;
            }
        }
        else if (isLockedOnPlayer)
        {
            // Go to one of the walkpoints if the player was seen previously, but with reaching this "if" the player was lost
            GoToNextPoint();
            isLockedOnPlayer = false;
        }
        else if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance + 1.0f)
        {
            // Agent has reached a walking point, 
            this.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.black;
            GoToNextPoint();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawWireSphere(this.transform.position, lookingDistance);
        Gizmos.DrawWireSphere(this.transform.position, alwaysFoundDistance);
        Gizmos.DrawWireSphere(this.transform.position, catchingDistance);
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
