using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }
    public OverlayMenu OverlayMenu;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PauseGameEvent.AddListener(StopTime);
        ResumeGameEvent.AddListener(StartTime);
        Init();
        //OverlayMenu.ShowDialog();
    }

    private void Init()
    {
        ResumeGameEvent.Invoke();
    }


    private void StartTime()
    {
        StartCoroutine(TogglePauseResume(0.1f, 1));
        IsPaused = false;
    }

    private void StopTime()
    {
        StartCoroutine(TogglePauseResume(0.1f, 0));
        IsPaused = true;
    }

    // Can hopefully help with NaN calculations (deviding by 0)
    private IEnumerator TogglePauseResume(float lerpDuration, int targetTimeScale)
    {
        float currentTimeScale = Time.timeScale;
        float elapsedTime = 0f;

        // stop time by duration 
        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // This will work even when delta time is 0
            Time.timeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, elapsedTime / lerpDuration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;
    }

    private void OnDestroy() 
    { 
        if (this == Instance)
        {
            Instance = null;  
        }
    }

    public bool OnElevator = false;
    public bool IsVisionConeOnPlayer = false;
    public bool isLockedOnWitchHead = false;
    public bool IsPaused = false;

    // Custom Events
    [HideInInspector]
    public UnityAction AllItemsCollectedEvent;
    [HideInInspector]
    public UnityAction GameOverEvent;
    [HideInInspector]
    public UnityEvent ResumeMovementEvent;
    [HideInInspector]
    public UnityEvent PauseMovementEvent;
    [HideInInspector]
    public UnityEvent PauseGameEvent;
    [HideInInspector]
    public UnityEvent ResumeGameEvent;
    [HideInInspector]
    public UnityEvent RespawnPlayerEvent;
    [HideInInspector]
    public UnityEvent NewCheckpointEvent;

    public delegate void DealDamageCallBack(int damage);
    public DealDamageCallBack DealDamageEvent;

    public delegate void ItemPickupCallBack(ItemData item);
    public ItemPickupCallBack ItemPickupEvent;

    public delegate void UsedItemCallBack(ItemData item);
    public UsedItemCallBack UsedItemEvent;
}
