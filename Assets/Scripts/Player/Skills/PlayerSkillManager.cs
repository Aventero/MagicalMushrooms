using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerSkill activeSkill = null;
    private SmokeBomb smokeBomb;
    private Poltergeist poltergeist;

    private bool lockSkills = false;

    public void Start()
    {
        smokeBomb = GetComponent<SmokeBomb>();
        poltergeist = GetComponent<Poltergeist>();

        smokeBomb.activated = true;
        poltergeist.activated = true;

        StateManager.Instance.PauseGameEvent.AddListener(this.OnPause);
        StateManager.Instance.ResumeGameEvent.AddListener(this.OnResume);
        StateManager.Instance.StartSlurpingEvent.AddListener(this.OnPause);
        StateManager.Instance.EndSlurpingEvent.AddListener(this.OnResume);
    }

    private void Update()
    {
        CheckSkillCost(poltergeist);
        CheckSkillCost(smokeBomb);
    }

    private void CheckSkillCost(PlayerSkill playerSkill)
    {
        if (playerSkill.isRecharging || lockSkills)
            return;

        int coins = Stats.Instance.CoinsCollected;

        if (coins >= playerSkill.SkillCost && !playerSkill.activated)
        {
            playerSkill.activated = true;
            UIManager.Instance.EnableSkill(playerSkill);
        }
        else if (coins < playerSkill.SkillCost && playerSkill.activated)
        {
            playerSkill.activated = false;
            UIManager.Instance.DisableSkill(playerSkill);
        }
    }

    public void OnPause()
    {
        lockSkills = true;
        if (activeSkill != null)
        {
            UIManager.Instance.SkillDeactivated();
            activeSkill.HidePreview();
            activeSkill = null;
        }
    }

    public void OnResume()
    {
        lockSkills = false;
    }

    public bool AreSkillsLocked()
    {
        return lockSkills;
    }

    public void LockSkills()
    {
        lockSkills = true;

        smokeBomb.activated = false;
        poltergeist.activated = false;

        UIManager.Instance.DisableSkill(poltergeist);
        UIManager.Instance.DisableSkill(smokeBomb);
    }
    public void UnlockSkills()
    {
        lockSkills = false;
    }

    public void SkillActivation(InputAction.CallbackContext callback)
    {
        if (activeSkill == null || lockSkills)
            return;

        // Check if the active skill can be held
        if (callback.performed)
        {
            if (!activeSkill.Execute())
                return;

            activeSkill.isRecharging = true;
            UIManager.Instance.SkillExecuted(activeSkill);
            StartCoroutine(this.LockSkillForSeconds(activeSkill, activeSkill.RechargeTime));
            activeSkill = null;
        }
    }

    public void OnPoltergeist(InputAction.CallbackContext callback)
    {
        if (!callback.performed || !poltergeist.activated || lockSkills)
            return;
        
        Preview(poltergeist);
    }

    public void OnSmokeBomb(InputAction.CallbackContext callback)
    {
        if (!callback.performed || !smokeBomb.activated || lockSkills)
            return;

        Preview(smokeBomb);
    }

    private void Preview(PlayerSkill skill)
    {
        if (activeSkill == null)
        {
            activeSkill = skill;
            activeSkill.ShowPreview();
            UIManager.Instance.SkillActivated(skill);
        }
        else if (activeSkill == skill)
        {
            activeSkill.HidePreview();
            activeSkill = null;
            UIManager.Instance.SkillDeactivated();
        }
    }

    private IEnumerator LockSkillForSeconds(PlayerSkill playerSkill, float time)
    {
        playerSkill.activated = false;
        yield return new WaitForSeconds(time);

        playerSkill.isRecharging = false;
    }
}
