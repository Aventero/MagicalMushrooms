using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    private SmokeBomb smokeBomb;

    private void Start()
    {
        smokeBomb = GetComponent<SmokeBomb>();
    }

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

        Debug.Log("Activating Smoke bomb");

        if (smokeBomb.IsActivated)
            smokeBomb.Deactivate();
        else
            smokeBomb.Activate();
    }

    public void OnMagicCloak(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;

        Debug.Log("Activating Magic cloak");
    }
}
