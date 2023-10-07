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

    // Watching
    public Transform Player;
    public GameObject WatchPointsParent;
    public GameObject VisionCone;
    public GameObject StandardWatchpoint;
    public List<Transform> WatchPoints { get; private set; }

    // Watching
    public Vector3 WatchTarget { get => Vision.currentWatchTarget; }
    public UnityAction ReachedDestination;
    public AIVision Vision { get; private set; }
    public AIMovement Movement {  get; private set; }
    public DangerOverlay DangerOverlay { get; private set; }

    // Animation
    public WitchUIAnimation UIAnimation { get; private set; }
    public PlayerDetection PlayerDetection { get; private set; }
    void Awake()
    {
        PlayerDetection = GetComponent<PlayerDetection>();
        Movement = GetComponent<AIMovement>();
        DangerOverlay = GetComponent<DangerOverlay>();
        Vision = GetComponent<AIVision>();
        UIAnimation = GetComponent<WitchUIAnimation>();

        // Get the Points from Parents
        WatchPoints = new List<Transform>(WatchPointsParent.GetComponentsInChildren<Transform>().Where(Point => Point != WatchPointsParent.transform));

        states.Add(AIStates.Idle, GetComponent<AIStateIdle>());
        states.Add(AIStates.Patrol, GetComponent<AIStatePatrol>());
        states.Add(AIStates.Chase, GetComponent <AIStateChase>());
        states.Add(AIStates.Attack, GetComponent<AIStateAttack>());
        states.Add(AIStates.IgnorePlayerIdle, GetComponent<AIStateIgnorePlayerIdle>());
        states.Add(AIStates.LostPlayer, GetComponent<AIStateLostPlayer>());
        states.Add(AIStates.Levitate, GetComponent<AIStateLevitate>());
        states.Add(AIStates.Capture, GetComponent<AIStateCapture>());
        states.Add(AIStates.SpottetPlayer, GetComponent<AIStateSpottedPlayer>());
        states.Add(AIStates.PanicSearch, GetComponent<AIStatePanicSearch>());
        states.Add(AIStates.Turn, GetComponent<AIStateTurn>());

        foreach (var state in states)
            state.Value.InitState(this);

        currentState = states[AIStates.Idle];
    }
    private void Start()
    {
        StartCoroutine(StartDelayed());
    }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        currentState.EnterState();
    }

    void Update()
    {
        if (StateManager.Instance.IsPaused)
            return;
        currentState.UpdateState();
        Vision.WatchCurrentTarget();
        Movement.AnimateWitch();
        DangerOverlay.UpdateColors();
        UIAnimation.UpdateAnimationStates();
    }



    public void Watch(Vector3 point)
    {
        Vision.Watch(point);
    }

    public bool HasFoundPlayer()
    {
        return Vision.FoundPlayer();
    }

    public bool HasLostPlayer()
    {
        return Vision.LostPlayer();
    }

    public void TransitionToState(AIStates stateName)
    {
        Debug.Log("New State: " + stateName);
        previousState = currentState;
        currentState.ExitState();
        currentState = states[stateName];
        currentState.EnterState();
    }

    public List<Transform> VisiblePointsAroundPlayer(Vector3 desiredPoint, Vector3 forward, float viewAngle)
    {
        List<Transform> visibleWatchPoints = new List<Transform>();
        foreach (Transform watchPoint in PlayerDetection.GetViewPointsAroundPlayer())
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

    public List<Transform> VisiblePointsFromWitchView(Vector3 desiredPoint, Vector3 forward, float viewAngle)
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

    public float EasyAngle(Vector3 position, Vector3 forward, Vector3 desiredPoint)
    {
        Vector2 forward2D = new Vector2(forward.x, forward.z);
        Vector2 towardsPoint = new Vector2(desiredPoint.x, desiredPoint.z) - new Vector2(position.x, position.z);
        return Vector2.Angle(forward2D, towardsPoint);
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
    PanicSearch,
    Patrol,
    RangeAttack,
    SpottetPlayer,
    Turn
}