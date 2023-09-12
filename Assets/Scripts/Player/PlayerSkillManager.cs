using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    public void OnPoltergeist(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;
        
        Debug.Log("Activating Poltergeist");
    }


    public void OnSmokeBomb(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;

        SmokeBomb smokeBomb = new();

        //TODO: Do smokebomb stuff
        Debug.Log("Activating Smoke bomb");
    }

    public void OnMagicCloak(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;

        Debug.Log("Activating Magic cloak");
    }
}
