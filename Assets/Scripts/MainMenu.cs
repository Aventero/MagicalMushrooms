using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public bool PlayerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerInRange)
            return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!StateManager.Instance.InMenu)
                ActivateMenu();
            else
                DeactivateMenu();
        }
    }

    public void OnButtonClick()
    {
        Debug.Log("Clicked On Button!");
    }

    private void ActivateMenu()
    {
        StateManager.Instance.InMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DeactivateMenu()
    {
        StateManager.Instance.InMenu = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
            PlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
            PlayerInRange = false;
    }
}
