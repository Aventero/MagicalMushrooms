using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface AIState
{
    public string StateName { get; }

    void UpdateState(AIStateManager stateManager);
    

    void EnterState(AIStateManager stateManager);

    
    void ExitState(AIStateManager stateManager);
}
