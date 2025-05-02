using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIControllerManager : MonoBehaviour
{
    public Stack<GameObject> activeButtons = new();
    private static UIControllerManager _instance;
    
    public static UIControllerManager Instance
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

    private void Start()
    {
        ControllerManager.OnControllerActivated += () =>
        {
            Debug.LogWarning("UIManager: OnControllerActivated: " + activeButtons.Count.ToString());
            
            if (activeButtons.Count > 0)
                Debug.LogWarning("Activating: " + activeButtons.Peek().name);
;
            SelectActiveButton();
        };
    }

    public void OpenSubmenu(GameObject firstSelection)
    {
        if(ControllerManager.Instance.IsControllerActive())
            OnPointerEnter(firstSelection);
        else
            activeButtons.Push(firstSelection);

        Debug.LogWarning("Opened Submenu with button: " + firstSelection.name + " ( " + activeButtons.Count + " )");
    }

    public void CloseSubmenu()
    {
        if(activeButtons.Count <= 0)
            return;

        activeButtons.Pop();
        
        Debug.LogWarning("Closing Submenu ( " + activeButtons.Count + " )");

        if (activeButtons.Count > 0 && ControllerManager.Instance.IsControllerActive()){
            activeButtons.Peek().GetComponent<Selectable>().Select();
            Debug.LogWarning("Closing Submenu now selecting: " + activeButtons.Peek().name);                        
        }
    }
    
    public void OnPointerEnter(GameObject button)
    {
        Debug.LogWarning("OnPointerEnter with " + button.name);
        if(!ControllerManager.Instance.IsControllerActive())
            return;
        
        button.GetComponent<Selectable>().Select();
            
        if (activeButtons.Count > 0)
            if (activeButtons.Peek().gameObject == button)
                return;
        
        activeButtons.Push(button);
        Debug.LogWarning("OnPointerEnter: " + activeButtons.Count);
    }

    public void OnPointerExit(GameObject button)
    {
        if(!ControllerManager.Instance.IsControllerActive())
            return;

        activeButtons.Pop();
        Debug.LogWarning("OnPointerExit: " + activeButtons.Count);
    }

    public void SelectActiveButton() {
        Debug.LogWarning("Selecting Active Button: " + activeButtons.Peek().name);
        if (activeButtons.Count > 0)
            activeButtons.Peek().GetComponent<Selectable>().Select();
        else
            Debug.LogError("<UIManager> SelectActiveButton Count is ZERO");
    }
    
    // TODO: Fix this method
    public void OnNavigate(InputAction.CallbackContext context) {
        Vector2 direction = context.ReadValue<Vector2>();
        Debug.Log("Navigation: " + direction + " Count: " + activeButtons.Count + " Name:" + activeButtons.Peek().name);
        
        Selectable currentSelectable = activeButtons.Peek().GetComponent<Selectable>();
        Selectable nextSelectedObject = null;

        
        if(direction.y >= 1)
            nextSelectedObject = currentSelectable.navigation.selectOnUp;
        else if(direction.y <= -1)
            nextSelectedObject = currentSelectable.navigation.selectOnDown;
        
        if(direction.x >= 1)
            nextSelectedObject = currentSelectable.navigation.selectOnRight;
        else if(direction.y <= -1)
            nextSelectedObject = currentSelectable.navigation.selectOnLeft;
        
        Debug.Log(nextSelectedObject);
        
        if(nextSelectedObject == null || nextSelectedObject.gameObject)
            return;
        
        OnPointerExit(currentSelectable.gameObject);
        OnPointerEnter(nextSelectedObject.gameObject);
    }
}