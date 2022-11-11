using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIMenu : MonoBehaviour
{
    private bool PlayerInRange = false;

    // Update is called once per frame
    void Update()
    {
        if (!PlayerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!StateManager.Instance.InMenu)
                ActivateMenu();
            else
                DeactivateMenu();
        }
    }

    private void ActivateMenu()
    {
        UIManager.Instance.SetOverlayVisibility = false;
        StateManager.Instance.InMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DeactivateMenu()
    {
        UIManager.Instance.SetOverlayVisibility = true;
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
