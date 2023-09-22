using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerSkill activeSkill = null;
    private SmokeBomb smokeBomb;
    private Poltergeist poltergeist;

    private bool lockSkill = false;

    private void Start()
    {
        smokeBomb = GetComponent<SmokeBomb>();
        poltergeist = GetComponent<Poltergeist>();
    }

    public void SkillActivation(InputAction.CallbackContext callback)
    {
        if (!callback.performed || activeSkill == null || lockSkill)
            return;

        activeSkill.Execute();
        lockSkill = true;
        StartCoroutine(this.LockSkillForSeconds(activeSkill.rechargeTime));

        activeSkill = null;
    }

    public void OnPoltergeist(InputAction.CallbackContext callback)
    {
        if (!callback.performed || lockSkill)
            return;
        
        Debug.Log("Activating Poltergeist");
        activeSkill = poltergeist;

        if (activeSkill.IsActivated)
            activeSkill.HidePreview();
        else
            activeSkill.ShowPreview();
    }

    public void OnSmokeBomb(InputAction.CallbackContext callback)
    {
        if (!callback.performed || lockSkill)
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
        if (!callback.performed || lockSkill)
            return;

        Debug.Log("Activating Magic cloak");
    }

    IEnumerator LockSkillForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        lockSkill = false;
        Debug.Log("Skill unlocked!");
    }
}
