using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Debug.Log("destorying");
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
        // Delay the start cause Events aren't loaded in yet.
        StartCoroutine(DelayedStart());
        //if (!FirstTimeLoad())
        //    return;
        // SetAlreadyPlayedGame();
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        FindObjectOfType<OverlayMenu>().ShowDialog();
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

    // Witch
    public bool WitchConeOnPlayer = false;

    // Camera
    public bool isLockedOnWitchHead = false;

    public bool InMenu = false;

    // Custom Events
    public UnityAction AllItemsCollectedEvent;
    public UnityAction GameOverEvent;
    public UnityAction PauseGameEvent;
    public UnityAction ResumeGameEvent;

    public delegate void DealDamageCallBack(int damage);
    public DealDamageCallBack DealDamageEvent;

    public delegate void ItemPickupCallBack(ItemData item);
    public ItemPickupCallBack ItemPickupEvent;

    public delegate void UsedItemCallBack(ItemData item);
    public UsedItemCallBack UsedItemEvent;

}
