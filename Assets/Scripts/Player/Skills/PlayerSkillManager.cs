using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerSkill activeSkill = null;
    private SmokeBomb smokeBomb;
    private Dragging dragging;
    private Poltergeist poltergeist;
    public static PlayerSkillManager Instance { get; private set; }

    private bool lockSkills = false;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        smokeBomb = GetComponent<SmokeBomb>();
        poltergeist = GetComponent<Poltergeist>();
        dragging = GetComponent<Dragging>();

        smokeBomb.activated = true;
        poltergeist.activated = true;
        dragging.activated = true;

        StateManager.Instance.PauseGameEvent.AddListener(this.OnPause);
        StateManager.Instance.ResumeGameEvent.AddListener(this.OnResume);
        StateManager.Instance.StartSlurpingEvent.AddListener(this.OnPause);
        StateManager.Instance.EndSlurpingEvent.AddListener(this.OnResume);
    }

    private void Update()
    {
        CheckSkillCost(poltergeist);
        CheckSkillCost(smokeBomb);
        CheckSkillCost(dragging);
    }

    private void CheckSkillCost(PlayerSkill playerSkill)
    {
        if (playerSkill.isRecharging || lockSkills)
            return;

        int coins = Stats.Instance.CoinsCollected;

        if (coins >= playerSkill.SkillCost && !playerSkill.activated)
        {
            playerSkill.activated = true;
            UIManager.Instance.EnableSkillVisual(playerSkill);
        }
        else if (coins < playerSkill.SkillCost)
        {
            playerSkill.activated = false;
            UIManager.Instance.DisableSkillVisual(playerSkill);
        }
    }

    public void OnPause()
    {
        LockSkills();
        if (activeSkill != null)
        {
            UIManager.Instance.SkillDeactivated();
            activeSkill.HidePreview();
            activeSkill = null;
        }
    }

    public bool HasActiveSkill()
    {
        return activeSkill != null;
    }

    public void OnResume()
    {
        UnlockSkills();
    }

    public bool AreSkillsLocked()
    {
        return lockSkills;
    }

    public void DisableAllSkills()
    {
        Debug.Log("Disabling skills!");
        LockSkills();
        smokeBomb.activated = false;
        poltergeist.activated = false;
        dragging.activated = false;
    }

    public void LockSkills()
    {
        lockSkills = true;
        UIManager.Instance.DisableSkillVisual(poltergeist);
        UIManager.Instance.DisableSkillVisual(smokeBomb);
        UIManager.Instance.DisableSkillVisual(dragging);
    }

    public void UnlockSkills()
    {
        if (activeSkill != null)
            return;

        lockSkills = false;
        UIManager.Instance.EnableSkillVisual(poltergeist);
        UIManager.Instance.EnableSkillVisual(smokeBomb);
        UIManager.Instance.EnableSkillVisual(dragging);
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

            SkillCompleted();
        }
    }

    public void SkillCompleted()
    {
        activeSkill.isRecharging = true;
        UIManager.Instance.SkillExecuted(activeSkill);
        StartCoroutine(LockSkillForSeconds(activeSkill, activeSkill.RechargeTime));
        activeSkill = null;
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

    public void OnDragging(InputAction.CallbackContext callback)
    {
        if (!callback.performed || !dragging.activated || lockSkills)
            return;

        Preview(dragging);
    }

    private void Preview(PlayerSkill skill)
    {
        // Show new one
        if (activeSkill == null)
        {
            activeSkill = skill;
            activeSkill.ShowPreview();
            UIManager.Instance.SkillActivated(skill);
            return;
        }

        // Hide old skill
        if (activeSkill == skill)
        {
            activeSkill.HidePreview();
            activeSkill = null;
            UIManager.Instance.SkillDeactivated();
            return;
        }

        if (activeSkill != skill)
        {
            // Hide old skill
            activeSkill.HidePreview();
            activeSkill = null;
            UIManager.Instance.SkillDeactivated();

            // Show new one
            activeSkill = skill;
            activeSkill.ShowPreview();
            UIManager.Instance.SkillActivated(skill);
            return;
        }
    }

    private IEnumerator LockSkillForSeconds(PlayerSkill playerSkill, float time)
    {
        playerSkill.activated = false;
        yield return new WaitForSeconds(time);
        playerSkill.isRecharging = false;
    }
}
