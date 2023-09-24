using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AIStateManager : MonoBehaviour
{
    public IAIState currentState;
    public IAIState previousState;

    public delegate void OnStateEvent();
    public OnStateEvent Onevent;

    public Dictionary<AIStates, IAIState> states = new Dictionary<AIStates, IAIState>();

    // Player Target
    public Transform Player;

    // AI model
    public Animator animator { get; private set; }

    // Watching
    public GameObject WatchPointsParent;
    public GameObject VisionCone;
    public GameObject StandardWatchpoint;
    public List<Transform> WatchPoints { get; private set; }

    // Walking
    public NavMeshAgent agent { get; private set; }
    public GameObject WalkPointsParent;
    public Vector3 currentWalkPoint { get; private set; }
    private Vector3 previousWalkPoint;
    public List<Transform> walkPoints { get; private set; }

    public Vector3 WatchTarget { get => aiVision.currentWatchTarget; }
    public UnityAction ReachedDestination;

    // Watching
    public AIVision aiVision { get; private set; }


    public DangerOverlay DangerOverlay { get; private set; }

    // Animation
    public WitchUIAnimation UIAnimation { get; private set; }

    void Awake()
    {
        DangerOverlay = GetComponent<DangerOverlay>();
        aiVision = GetComponent<AIVision>();
        animator = GetComponent<Animator>();
        UIAnimation = GetComponent<WitchUIAnimation>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;

        // Get the Points from Parents
        walkPoints = new List<Transform>(WalkPointsParent.GetComponentsInChildren<Transform>().Where(point => point != WalkPointsParent.transform));
        WatchPoints = new List<Transform>(WatchPointsParent.GetComponentsInChildren<Transform>().Where(Point => Point != WatchPointsParent.transform));
        currentWalkPoint = previousWalkPoint = walkPoints[0].position;

        states.Add(AIStates.Idle, GetComponent<AIStateIdle>());
        states.Add(AIStates.Patrol, GetComponent<AIStatePatrol>());
        states.Add(AIStates.Chase, GetComponent <AIStateChase>());
        states.Add(AIStates.Attack, GetComponent<AIStateAttack>());
        states.Add(AIStates.IgnorePlayerIdle, GetComponent<AIStateIgnorePlayerIdle>());
        states.Add(AIStates.LostPlayer, GetComponent<AIStateLostPlayer>());
        states.Add(AIStates.Levitate, GetComponent<AIStateLevitate>());
        states.Add(AIStates.Capture, GetComponent<AIStateCapture>());
        states.Add(AIStates.SpottetPlayer, GetComponent<AIStateSpottedPlayer>());

        foreach (var state in states)
            state.Value.InitState(this);

        currentState = states[AIStates.Idle];
        currentState.EnterState();
    }

    public void SetWalkPoint(Vector3 point)
    {
        previousWalkPoint = currentWalkPoint;
        currentWalkPoint = point;
        agent.destination = currentWalkPoint;
    }

    public void Walk()
    {
        agent.isStopped = false;
    }

    public void StopAgent()
    {
        agent.isStopped = true;
    }

    void LateUpdate()
    {
        currentState.UpdateState();
        aiVision.WatchSpot();
        AnimateWitch();
        DangerOverlay.UpdateColors();
        UIAnimation.UpdateAnimationStates();
        Debug.DrawLine(transform.position, currentWalkPoint, Color.green);
        Debug.DrawLine(transform.position, previousWalkPoint, Color.white);
    }

    public void Watch(Transform point)
    {
        aiVision.Watch(point.position);
    }

    public void Watch(Vector3 point)
    {
        aiVision.Watch(point);
    }

    private void AnimateWitch()
    {
        if (agent.velocity.sqrMagnitude <= 0.1f)
            animator.SetBool("Stay", true);
        else
            animator.SetBool("Stay", false);
    }

    public bool HasFoundPlayer()
    {
        return aiVision.FoundPlayer();
    }

    public bool HasLostPlayer()
    {
        return aiVision.LostPlayer();
    }

    public void TransitionToState(AIStates stateName)
    {
        Debug.Log("New State: " + stateName);
        previousState = currentState;
        currentState.ExitState();
        currentState = states[stateName];
        currentState.EnterState();
    }

    public List<Transform> CalculateVisiblePoints(Vector3 desiredPoint, Vector3 forward, float viewAngle)
    {
        List<Transform> visibleWatchPoints = new List<Transform>();
        foreach (Transform watchPoint in WatchPoints)
        {
            float angle = EasyAngle(desiredPoint, forward, watchPoint.position);
            if (angle <= viewAngle)
            {
                visibleWatchPoints.Add(watchPoint);
                Debug.DrawLine(transform.position, watchPoint.position, Color.magenta, 4f);
            }
        }

        return visibleWatchPoints;
    }

    public Transform CalculateClosestNotVisiblePoint(Vector3 desiredPoint, Vector3 forward)
    {
        Transform closestPoint = WatchPoints.ElementAt(0);

        float shortestAngle = 360f;
        foreach (Transform watchPoint in WatchPoints)
        {
            float angleToWatchPoint = EasyAngle(desiredPoint, forward, watchPoint.position);
            if (angleToWatchPoint <= shortestAngle)
            {
                closestPoint = watchPoint;
                shortestAngle = angleToWatchPoint;
            }
        }

        return closestPoint;
    }

    public float EasyAngle(Vector3 position, Vector3 forward, Vector3 desiredPoint)
    {
        Vector2 forward2D = new Vector2(forward.x, forward.z);
        Vector2 towardsPoint = new Vector2(desiredPoint.x, desiredPoint.z) - new Vector2(position.x, position.z);
        return Vector2.Angle(forward2D, towardsPoint);
    }

    public Transform FindNewWalkpoint()
    {
        // No Walkpoints means no walking!
        if (walkPoints.Count == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return null;
        }

        // RANDOM WALK POINT
        // OR Better? -> Walkpoints that are around the player! Make the witch focus more on the sweet player!


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

        for (int i = 0; i < closestWalkPoints.Count / 2; i++)
            Debug.DrawLine(transform.position, closestWalkPoints[i].position, Color.yellow, 1f);

        return shortestPoint;
    }
}
public enum AIStates
{
    Attack,
    Capture,
    Chase,
    Idle,
    IgnorePlayerIdle,
    Levitate,
    LostPlayer,
    Patrol,
    RangeAttack,
    SpottetPlayer
}