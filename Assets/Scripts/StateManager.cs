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

    private readonly string AlreadyPlayedLevel = "AlreadyPlayedGame";

    private void Start()
    {
        PauseGameEvent.AddListener(StopTime);
        ResumeGameEvent.AddListener(StartTime);
        OverlayMenu.ShowDialog();

        //if (!FirstTimeLoad())
        //    return;
        // SetAlreadyPlayedGame();
    }


    private void StartTime()
    {
        Time.timeScale = 1;
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }

    private void OnDestroy() 
    { 
        if (this == Instance)
        {
            Debug.Log("Setting null");
            Instance = null;  
        }
    }

    private void SetAlreadyPlayedGame()
    {
        PlayerPrefs.SetInt(AlreadyPlayedLevel, 1);
        PlayerPrefs.Save();
    }

    private bool FirstTimeLoad()
    {
        return !PlayerPrefs.HasKey(AlreadyPlayedLevel);
    }

    public bool OnElevator = false;
    public bool WitchConeOnPlayer = false;
    public bool isLockedOnWitchHead = false;
    public bool InMenu = false;

    // Custom Events
    public UnityAction AllItemsCollectedEvent;
    public UnityAction GameOverEvent;
    [HideInInspector]
    public UnityEvent PauseGameEvent;
    [HideInInspector]
    public UnityEvent ResumeGameEvent;

    public delegate void DealDamageCallBack(int damage);
    public DealDamageCallBack DealDamageEvent;

    public delegate void ItemPickupCallBack(ItemData item);
    public ItemPickupCallBack ItemPickupEvent;

    public delegate void UsedItemCallBack(ItemData item);
    public UsedItemCallBack UsedItemEvent;
}
