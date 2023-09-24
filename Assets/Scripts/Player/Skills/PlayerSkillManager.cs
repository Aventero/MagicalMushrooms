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

        if (!activeSkill.Execute())
            return;

        lockSkill = true;
        StartCoroutine(this.LockSkillForSeconds(activeSkill.RechargeTime));
        activeSkill = null;
    }

    public void OnPoltergeist(InputAction.CallbackContext callback)
    {
        if (!callback.performed || lockSkill)
            return;
        
        Debug.Log("Activating Poltergeist");
        ActivateSkill(poltergeist);
    }

    public void OnSmokeBomb(InputAction.CallbackContext callback)
    {
        if (!callback.performed || lockSkill)
            return;

        Debug.Log("Activating Smoke bomb");
        ActivateSkill(smokeBomb);
    }

    public void OnMagicCloak(InputAction.CallbackContext callback)
    {
        if (!callback.performed || lockSkill)
            return;

        Debug.Log("Activating Magic cloak");
    }

    private void ActivateSkill(PlayerSkill skill)
    {
        if (activeSkill == null)
        {
            activeSkill = skill;
            activeSkill.ShowPreview();
        }
        else if (activeSkill == skill)
        {
            activeSkill.HidePreview();
            activeSkill = null;
        }
    }

    private IEnumerator LockSkillForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        lockSkill = false;
        Debug.Log("Skill unlocked!");
    }
}
