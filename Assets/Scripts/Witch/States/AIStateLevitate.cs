using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateLevitate : MonoBehaviour, IAIState
{
    private AIStateManager stateManager;
    public string StateName => "Levitate";
    public float levitationRadius = 10f;
    public float LevitationTime = 5f;

    public LayerMask targetLayer;
    public AIStateManager AIStateManager { get => stateManager; }

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        LevitateObjectPlayerIsHidingBehind();
        StartCoroutine(SearchAround());
    }

    public void ExitState()
    {
        
    }

    public void UpdateState()
    {
        if (stateManager.HasFoundPlayer())
        {
            StopAllCoroutines();
            stateManager.TransitionToState("Chase");
        }
    }

    private void LevitateObjectPlayerIsHidingBehind()
    {
        stateManager.aiVision.ObjectPlayerIsHidingBehind.GetComponent<Draggable>().Levitate();
    }

    IEnumerator SearchAround()
    {
        yield return new WaitForSeconds(LevitationTime);
        stateManager.TransitionToState("Patrol");
    }

    private GameObject FindObjectToLevitate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(stateManager.Player.position, levitationRadius, targetLayer);
        List<Collider> draggableColliders = new List<Collider>();

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Draggable"))
                draggableColliders.Add(col);
        }

        if (draggableColliders.Count == 0)
            return null;

        Collider closeCollider = draggableColliders.Find(col => Vector3.Distance(col.gameObject.transform.position, stateManager.Player.position) < levitationRadius);
        if (closeCollider == null)
            return null;
        
        GameObject closeObject = closeCollider.gameObject;
        closeObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f);
        return closeObject;
    }

}

