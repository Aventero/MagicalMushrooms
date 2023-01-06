using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WitchMovement : MonoBehaviour
{
    public Transform player;
    public Transform[] walkPoints;
    public Dictionary<Transform, int> WalkPointVisits;
    //public float lookingDistance = 5.0f;
    //public float lookingAngle = 45.0f;   // Angle from the forward vector
    //public float alwaysFoundDistance = 10.0f;
    public float catchingDistance = 5.5f;
    public int PlayerDamage = 1;

    private NavMeshAgent agent;
    public Transform currentWalkPoint { get; private set; }
    private Transform previousWalkPoint;
    public bool isLockedOnPlayer = false;
    public float secondsTillCaught = 3.0f;
    private float timeInsideCatchArea = 0.0f;

    Animator animator;
    //private bool pickingUpIsPlaying = false;
    public Transform witchCameraPosition;
    public Transform witchCameraTarget;
    private bool choosingNextDestination = false;
    private bool witchIsMoving = false;
    public float WaitingTime = 3f;
    public UnityAction HasReachedDestination;
    public UnityAction StartsMovingAgain;
    private WitchWatching witchWatching;

    public bool WitchIsMoving
    {
        get 
        {
            witchIsMoving = agent.remainingDistance >= agent.stoppingDistance + 1.0f;
            return witchIsMoving;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.autoBraking = true;
        currentWalkPoint = previousWalkPoint = walkPoints[0];
        WalkPointVisits = new Dictionary<Transform, int>();
        foreach (Transform point in walkPoints)
        {
            WalkPointVisits.Add(point, Random.Range(0, 2));
        }
        witchWatching = GetComponent<WitchWatching>();
        witchWatching.IsDoneWatching += GoToNextDestination;
    }

    private void GoToNextDestination()
    {
        // Next destination is set!
        choosingNextDestination = false;
        animator.SetBool("Stay", false);
        StartsMovingAgain.Invoke();
        agent.destination = currentWalkPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, currentWalkPoint.position, Color.green);
        Debug.DrawLine(transform.position, previousWalkPoint.position, Color.white);

        //// Once the player was too long in the area, play the animation
        //if (timeInsideCatchArea >= secondsTillCaught)
        //{
        //    timeInsideCatchArea = 0;
        //    animator.SetTrigger("PickUp");
        //    this.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
        //    StateManager.Instance.isLockedOnWitchHead = true;
        //    StateManager.Instance.DealDamageEvent(PlayerDamage);
        //    return;
        //}

        //// Don't move, while pickingUp or in transition to another animation
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("PickUpPlayer") || animator.IsInTransition(0))
        //{
        //    player.transform.position = witchCameraPosition.position;
        //    Camera.main.transform.LookAt(witchCameraTarget, Vector3.up);
        //    return;
        //}

       // StateManager.Instance.isLockedOnWitchHead = false;

        // On XZ Plane
        //float distance2DToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z));

        //// Get the angle on the xz plane, from agent to player (from 0 to 180)
        //float angleToPlayer = Vector2.Angle(new Vector2(agent.transform.forward.x, agent.transform.forward.z), new Vector2(player.position.x, player.position.z) - new Vector2(agent.transform.position.x, agent.transform.position.z));
        //float posAngle = Mathf.Deg2Rad * (agent.transform.localEulerAngles.y + lookingAngle);   // Angle in Radians
        //float negAngle = Mathf.Deg2Rad * (agent.transform.localEulerAngles.y - lookingAngle);   // -Angle in Radians
        //Debug.DrawLine(agent.transform.position, agent.transform.position + new Vector3(lookingDistance * Mathf.Sin(posAngle), 0, lookingDistance * Mathf.Cos(posAngle)), Color.green);
        //Debug.DrawLine(agent.transform.position, agent.transform.position + new Vector3(lookingDistance * Mathf.Sin(negAngle), 0, lookingDistance * Mathf.Cos(negAngle)), Color.yellow);

        // Raycast will only hit objects (Chair, Table, etc.), if it hits one and the player is behind it, don't move there
        //bool playerIsBehindObject = Physics.Raycast(agent.transform.position, (player.position - agent.transform.position), LayerMask.NameToLayer("Ignore Raycast"));

        //// player sees the player and is in range
        //if ((distance2DToPlayer < lookingDistance && angleToPlayer <= lookingAngle && !playerIsBehindObject) || distance2DToPlayer <= alwaysFoundDistance)
        //{
        //    Debug.DrawRay(agent.transform.position, (player.position - agent.transform.position), Color.cyan);
        //    agent.destination = player.position;
        //    GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        //    isLockedOnPlayer = true;

        //    // Player is inside the catching area
        //    if (distance2DToPlayer <= catchingDistance)
        //    {
        //        timeInsideCatchArea += Time.deltaTime;
        //    }
        //    else
        //    {
        //        // Reset the catching time, cause the player is not inside it
        //        timeInsideCatchArea = 0;
        //    }
        //}
        //else if (isLockedOnPlayer)
        //{
        //    // Go to one of the walkpoints if the player was seen previously, but with reaching this "if" the player was lost
        //    GoToNextPoint();
        //    isLockedOnPlayer = false;
        //}
        
        if (ReachedDestination())
        {
            // Agent has reached a walking point -> Choose the next one
            ChooseNextWalkPoint();
        }
    }

    private bool ReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance < agent.stoppingDistance && !isLockedOnPlayer && !choosingNextDestination;
    }

    public void GoToPlayer()
    {
        //float distance2DToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z));
        animator.SetBool("Stay", false);
        agent.destination = player.position;
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
        //Gizmos.DrawWireSphere(this.transform.position, lookingDistance);
        //Gizmos.DrawWireSphere(this.transform.position, alwaysFoundDistance);
        Gizmos.DrawWireSphere(this.transform.position, catchingDistance);
    }

    public void ChooseNextWalkPoint()
    {
        choosingNextDestination = true;
        animator.SetBool("Stay", true);
        StartCoroutine(WaitThenChoosePoint(WaitingTime));
    }

    IEnumerator WaitThenChoosePoint(float waitTime)
    {
        // Witch has visited this point!
        WalkPointVisits[currentWalkPoint]++;

        // No Walkpoints means no walking!
        if (walkPoints.Length == 0)
        {
            Debug.Log("No walkingpoints set up!");
            yield break;
        }

        // Get the shortest path with, wich is not the current & previous one!
        Transform shortestPoint = null;
        foreach (Transform t in walkPoints)
        {
            if (t != currentWalkPoint && t != previousWalkPoint)
            {
                if (shortestPoint == null)
                {
                    shortestPoint = t;
                    continue;
                }

                if (Vector3.Distance(transform.position, t.position) < Vector3.Distance(transform.position, shortestPoint.position) && WalkPointVisits[t] <= WalkPointVisits[shortestPoint])
                {
                    shortestPoint = t;
                }
            }
        }

        previousWalkPoint = currentWalkPoint;
        currentWalkPoint = shortestPoint;
        HasReachedDestination.Invoke();

    }
}
