using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


internal class AIStateAttack : MonoBehaviour, AIState
{
    public string StateName => "Attack";
    public TwoBoneIKConstraint HandIK;
    public MultiAimConstraint MultiAimHand;
    public GameObject PullPoint;
    public float ReachTime = 2f;
    public float PullSpeed = 2f;

    private Transform player;
    private bool pulling = false;

    public void InitState(AIStateManager stateManager)
    {
    }

    public void EnterState(AIStateManager stateManager)
    {
        stateManager.aiVision.PlayerWatching();
        player = stateManager.Player.transform;
        StartCoroutine(ReachOutHand(stateManager, ReachTime));   
    }

    public void ExitState(AIStateManager stateManager)
    {
        // Let Witch chill.
        StopAllCoroutines();
        player.gameObject.GetComponent<NewPlayerMovement>().ActivateMovement(true);
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
            MultiAimHand.weight = HandIK.weight =  Mathf.Lerp(0, 1, delta / reachTime);
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
            MultiAimHand.weight = HandIK.weight = Mathf.Lerp(1, 0, delta / reachTime);
            yield return null;
        }

        stateManager.TransitionToState("IgnorePlayerIdle");
    }

    IEnumerator PullToPullPoint()
    {
        // Slowly sucks the player toward the pulling point
        pulling = true;
        while (Vector3.Distance(PullPoint.transform.position, player.transform.position) > 1f)
        {
            CharacterController controller = player.gameObject.GetComponent<CharacterController>();
            controller.Move((PullPoint.transform.position - player.transform.position).normalized * Time.deltaTime * PullSpeed);
            yield return null;
        }
        pulling = false;
        yield return null;
    }


}