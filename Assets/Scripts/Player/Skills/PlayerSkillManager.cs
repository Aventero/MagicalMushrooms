using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerSkill activeSkill = null;
    private SmokeBomb smokeBomb;
    private Poltergeist poltergeist;
    private CoinVacuum coinVacuum;

    private bool lockSkill = false;

    private void Start()
    {
        smokeBomb = GetComponent<SmokeBomb>();
        coinVacuum = GetComponent<CoinVacuum>();
        poltergeist = GetComponent<Poltergeist>();

        StateManager.Instance.PauseGameEvent.AddListener(this.OnPause);
        StateManager.Instance.ResumeGameEvent.AddListener(this.OnResume);
    }

    public void OnPause()
    {
        lockSkill = true;
        if (activeSkill != null)
        {
            UIManager.Instance.SkillDeactivated();
            activeSkill.HidePreview();
            activeSkill = null;
        }
    }

    public void OnResume()
    {
        lockSkill = false;
    }

    public void SkillActivation(InputAction.CallbackContext callback)
    {
        if (!callback.performed || activeSkill == null || lockSkill)
            return;

        if (!activeSkill.Execute())
            return;

        lockSkill = true;
        UIManager.Instance.SkillExecuted(activeSkill);
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

    public void OnCoinVacuum(InputAction.CallbackContext callback)
    {
        if (!callback.performed || lockSkill)
            return;

        Debug.Log("Activating Coin vacuum");
        ActivateSkill(coinVacuum);
    }

    private void ActivateSkill(PlayerSkill skill)
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

    private IEnumerator LockSkillForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        lockSkill = false;
        Debug.Log("Skill unlocked!");
    }
}
