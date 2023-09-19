using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerSkill activeSkill = null;
    private SmokeBomb smokeBomb;
    private Poltergeist poltergeist;

    private void Start()
    {
        smokeBomb = GetComponent<SmokeBomb>();
        poltergeist = GetComponent<Poltergeist>();
    }

    public void SkillActivation(InputAction.CallbackContext callback)
    {
        if (!callback.performed || activeSkill == null)
            return;

        // TODO: Maybe check if the skill has the execute method?

        activeSkill.Execute();
    }

    public void OnPoltergeist(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;
        
        Debug.Log("Activating Poltergeist");
        activeSkill = poltergeist;
    }

    public void OnSmokeBomb(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;

        Debug.Log("Activating Smoke bomb");

        activeSkill = smokeBomb;

        if (smokeBomb.IsActivated)
            smokeBomb.HidePreview();
        else
            smokeBomb.ShowPreview();
    }

    public void OnMagicCloak(InputAction.CallbackContext callback)
    {
        if (!callback.performed)
            return;

        Debug.Log("Activating Magic cloak");
    }
}
