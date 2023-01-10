using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


internal class AIStateAttack : MonoBehaviour, AIState
{
    public string StateName => "Attack";
    public TwoBoneIKConstraint HandIK;
    public GameObject HandTarget;

    public float ReachTime = 2f;
    private Transform target;

    public void EnterState(AIStateManager stateManager)
    {
        Debug.Log("Attack");
        target = stateManager.Player.transform;
        HandTarget.transform.position = target.position;
        StartCoroutine(ReachOutHand(stateManager, ReachTime));   
    }

    public void ExitState(AIStateManager stateManager)
    {
    }

    public void UpdateState(AIStateManager stateManager)
    {
    }


    IEnumerator ReachOutHand(AIStateManager stateManager, float reachTime)
    {
        float delta = 0;

        while (delta <= reachTime)
        {
            delta += Time.deltaTime;
            HandIK.weight =  Mathf.Lerp(0, 1, delta / reachTime);
            yield return null;
        }

        HandIK.weight = 0;
        StateManager.Instance.DealDamageEvent(1);
        yield return new WaitForSeconds(reachTime);
        stateManager.TransitionToState("Patrol");
    }
}