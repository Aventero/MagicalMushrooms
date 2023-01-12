using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIStateManager : MonoBehaviour
{
    public AIState currentState;
    public AIState previousState;

    public delegate void OnStateEvent();
    public OnStateEvent Onevent;

    public Dictionary<string, AIState> states = new Dictionary<string, AIState>();

    // Player Target
    public Transform Player;

    // AI model
    public Animator animator { get; private set; }

    // Watching
    public GameObject WatchPointsParent;
    public GameObject ViewCone;
    public GameObject StandardWatchpoint;
    public List<Transform> WatchPoints { get; private set; }

    // Walking
    public NavMeshAgent agent { get; private set; }
    public GameObject WalkPointsParent;
    public Transform currentWalkPoint { get; private set; }
    private Transform previousWalkPoint;
    public List<Transform> walkPoints { get; private set; }

    public Transform WatchTarget { get => aiVision.currentWatchTarget; }

    // Watching
    public AIVision aiVision { get; private set; }

    void Awake()
    {
        aiVision = GetComponent<AIVision>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;

        // Get the Points from Parents
        walkPoints = new List<Transform>(WalkPointsParent.GetComponentsInChildren<Transform>().Where(point => point != WalkPointsParent.transform));
        WatchPoints = new List<Transform>(WatchPointsParent.GetComponentsInChildren<Transform>().Where(Point => Point != WatchPointsParent.transform));
        currentWalkPoint = previousWalkPoint = walkPoints[0];

        states.Add("Idle", GetComponent<AIStateIdle>());
        states.Add("Patrol", GetComponent<AIStatePatrol>());
        states.Add("Chase", GetComponent <AIStateChase>());
        states.Add("Attack", GetComponent<AIStateAttack>());
        states.Add("IgnorePlayerIdle", GetComponent<AIStateIgnorePlayerIdle>());

        foreach (var state in states)
            state.Value.InitState(this);

        aiVision.HasFoundPlayer += TransitionToChase;
        aiVision.HasLostPlayer += TransitionToPatrol;

        currentState = states["Idle"];
        currentState.EnterState(this);
    }

    private void TransitionToPatrol()
    {
        if (currentState.StateName == "Chase")
            TransitionToState("Patrol");
    }

    private void TransitionToChase()
    {
        if (currentState.StateName == "Idle" || currentState.StateName == "Patrol")
            TransitionToState("Chase");
    }

    void Update()
    {
        currentState.UpdateState(this);
        aiVision.WatchSpot();

        Debug.DrawLine(transform.position, currentWalkPoint.position, Color.green);
        Debug.DrawLine(transform.position, previousWalkPoint.position, Color.white);
    }

    public void Watch(Transform point)
    {
        aiVision.Watch(point);
    }

    public void TransitionToState(string stateName)
    {
        previousState = currentState;
        currentState.ExitState(this);
        currentState = states[stateName];
        currentState.EnterState(this);
        Debug.Log(currentState.StateName);
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
                Debug.DrawLine(transform.position, watchPoint.position, Color.magenta, 2f);
            }
        }

        return visibleWatchPoints;
    }

    public float EasyAngle(Vector3 position, Vector3 forward, Vector3 desiredPoint)
    {
        Vector2 forward2D = new Vector2(forward.x, forward.z);
        Vector2 towardsPoint = new Vector2(desiredPoint.x, desiredPoint.z) - new Vector2(position.x, position.z);
        return Vector2.Angle(forward2D, towardsPoint);
    }

    public void FindNewWalkpoint()
    {
        // No Walkpoints means no walking!
        if (walkPoints.Count == 0)
        {
            Debug.Log("No walkingpoints set up!");
            return;
        }

        // Get the shortest path with, wich is not the current & previous one!
        List<Transform> closestWalkPoints = new List<Transform>();
        foreach (Transform walkPoint in walkPoints)
        {
            if (walkPoint != currentWalkPoint && walkPoint != previousWalkPoint)
            {
                closestWalkPoints.Add(walkPoint);
            }
        }
        closestWalkPoints.Sort((p1, p2) => Vector3.Distance(transform.position, p1.position).CompareTo(Vector3.Distance(transform.position, p2.position)));

        // Randomly choose one of the 5 closest points
        Transform shortestPoint = closestWalkPoints[Random.Range(0, closestWalkPoints.Count / 2)];

        for (int i = 0; i < closestWalkPoints.Count / 2; i++)
            Debug.DrawLine(transform.position, closestWalkPoints[i].position, Color.yellow, 1f);

        previousWalkPoint = currentWalkPoint;
        currentWalkPoint = shortestPoint;
    }

    public IEnumerator FindWatchpointForPatrol()
    {
        // No Watchpoints means no watching!
        if (WatchPoints.Count == 0)
        {
            Debug.Log("No walkingpoints set up!");
            yield break;
        }

        // Choose one randomly
        Vector3 forwardToWalkpoint = currentWalkPoint.position - transform.position;
        List<Transform> visiblePointsAtNextDestination = CalculateVisiblePoints(currentWalkPoint.position, forwardToWalkpoint, 75f);
        if (visiblePointsAtNextDestination.Count == 0)
        {
            Watch(StandardWatchpoint.transform);
        }
        else
            Watch(visiblePointsAtNextDestination[UnityEngine.Random.Range(0, visiblePointsAtNextDestination.Count)]);

        if (EasyAngle(transform.position, transform.forward, aiVision.currentWatchTarget.position) > 75f)
        {
            // the new point is behind the witch, look at the standard and then at the corrent one
            Transform tmp = aiVision.currentWatchTarget;
            Watch(StandardWatchpoint.transform);
            yield return new WaitUntil(() => EasyAngle(transform.position, transform.forward, currentWalkPoint.position) < 45f);
            Watch(tmp);
        }

        yield return null;
    }
}
