using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }
    public OverlayMenu OverlayMenu;
    public PlayerInput PlayerInput;


    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PauseGameEvent.AddListener(StopTime);
        PauseGameEvent.AddListener(UnlockMouse);

        ResumeGameEvent.AddListener(StartTime);
        ResumeGameEvent.AddListener(LockMouse);
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

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
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
    public bool IsAllowedToSeePlayer = true;
    public bool isLockedOnWitchHead = false;
    public bool IsPaused = false;

    // Custom Events
    [HideInInspector]
    public UnityAction AllItemsCollectedEvent;
    [HideInInspector]
    public UnityAction GameOverEvent;
    [HideInInspector]
    public UnityEvent ResumePlayerMovementEvent;
    [HideInInspector]
    public UnityEvent ResumePlayerCameraEvent;
    [HideInInspector]
    public UnityEvent PausePlayerMovementEvent;
    [HideInInspector]
    public UnityEvent PausePlayerCameraEvent;
    [HideInInspector]
    public UnityEvent PauseGameEvent;
    [HideInInspector]
    public UnityEvent ResumeGameEvent;
    [HideInInspector]
    public UnityEvent RespawnPlayerEvent;
    [HideInInspector]
    public UnityEvent NewCheckpointEvent;
    [HideInInspector]
    public UnityEvent StartedDialogEvent;
    [HideInInspector]
    public UnityEvent EndedDialogEvent;
    [HideInInspector]
    public UnityEvent StartSlurpingEvent;
    [HideInInspector]
    public UnityEvent EndSlurpingEvent;



    public delegate void DealDamageCallBack(int damage);
    public DealDamageCallBack DealDamageEvent;

    public delegate void ItemPickupCallBack(ItemData item);
    public ItemPickupCallBack ItemPickupEvent;

    public delegate void UsedItemCallBack(ItemData item);
    public UsedItemCallBack UsedItemEvent;
}
