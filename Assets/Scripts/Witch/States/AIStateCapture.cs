using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateCapture : MonoBehaviour, IAIState
{
    public string StateName => "Capture";
    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;


    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public void EnterState()
    {
        // Spawn Cup
    }

    public void ExitState()
    {
        // Delete Cup
    }

    public void UpdateState()
    {
        // Hover Cup above player
        // Once time ran out, capture player
    }

}
