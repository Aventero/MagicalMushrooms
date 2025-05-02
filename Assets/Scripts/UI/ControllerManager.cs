using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    private bool activatedController;
    public static event UnityAction OnControllerActivated; 
    
    private static ControllerManager _instance;
    public static ControllerManager Instance
    {
        get
        {
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // Destroy the entire GameObject, not just the component
        }
        else
        {
            _instance = this;
        }
    }

    public void ActivateGamepad()
    {
        activatedController = true;
        Cursor.visible = false;
        
        OnControllerActivated.Invoke();
    }

    public void DeactivateGamepad()
    {
        activatedController = false;
        Cursor.visible = true;
    }

    public bool IsControllerActive()
    {
        return activatedController;
    }
    
    public void OnDeviceChangedEvent(PlayerInput input) {
        Debug.Log("Now using " + input.currentControlScheme);
        
        switch(input.currentControlScheme) {
            case "Keyboard&Mouse":
                activatedController = false;
                DeactivateGamepad();
                break;
            case "Gamepad":
                activatedController = true;
                ActivateGamepad();
                break;
        }
    }
}
