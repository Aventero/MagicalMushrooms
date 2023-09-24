using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateRangeAttack : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.RangeAttack;

    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }



    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
