﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


internal class AIStateAttack : MonoBehaviour, AIState
{
    public string StateName => "Attack";
    public TwoBoneIKConstraint HandIK;
    public GameObject HandTarget;

    public float ReachTime = 2f;
    private Transform player;
    private bool pulling = false;
    public GameObject PullPoint;

    public void EnterState(AIStateManager stateManager)
    {
        Debug.Log("Attack");
        player = stateManager.Player.transform;
        HandTarget.transform.position = PullPoint.transform.position;
        StartCoroutine(ReachOutHand(stateManager, ReachTime));   
    }

    public void ExitState(AIStateManager stateManager)
    {        
        // Let Witch chill.
        stateManager.StopHunting(5f);
    }

    public void UpdateState(AIStateManager stateManager)
    {
    }


    IEnumerator ReachOutHand(AIStateManager stateManager, float reachTime)
    {
        // Activate playermovement
        player.gameObject.GetComponent<NewPlayerMovement>().ActivateMovement(false);

        // Lerp hand to Pullpoint
        float delta = 0;
        while (delta <= reachTime)
        {
            delta += Time.deltaTime;
            HandIK.weight =  Mathf.Lerp(0, 1, delta / reachTime);
            yield return null;
        }

        // Start pulling the player
        StartCoroutine(PullToPullPoint());
        yield return new WaitForSeconds(reachTime);
        yield return new WaitUntil(() => pulling == false);

        // Pulling was done!
        StateManager.Instance.DealDamageEvent(1);
        player.gameObject.GetComponent<NewPlayerMovement>().ActivateMovement(true);

        // Lerp hand back
        delta = 0;
        while (delta <= reachTime)
        {
            delta += Time.deltaTime;
            HandIK.weight = Mathf.Lerp(1, 0, delta / reachTime);
            yield return null;
        }

        stateManager.TransitionToState("Patrol");
    }

    IEnumerator PullToPullPoint()
    {
        // Slowly sucks the player toward the pulling point
        pulling = true;
        while (Vector3.Distance(PullPoint.transform.position, player.transform.position) > 1f)
        {
            CharacterController controller = player.gameObject.GetComponent<CharacterController>();
            controller.Move((PullPoint.transform.position - player.transform.position).normalized * Time.deltaTime);
            yield return null;
        }
        pulling = false;
        yield return null;
    }

}