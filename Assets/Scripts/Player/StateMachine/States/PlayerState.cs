using UnityEngine;

public abstract class PlayerState
{
    public string Name { get; }
    protected bool isRootState = false; 
    protected PlayerStateMachine context;
    protected PlayerStateFactory factory;
    protected PlayerState currentSubState;
    protected PlayerState currentSuperState;

    public PlayerState(PlayerStateMachine context, PlayerStateFactory factory, string name)
    {
        this.context = context;
        this.factory = factory;
        Name = name;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() 
    {
        UpdateState();
        if (currentSubState != null)
            currentSubState.UpdateStates();
    }

    public void SwitchState(PlayerState newState) 
    {
        // Exit current state
        ExitState();

        newState.EnterState();

        if (isRootState)
        {
            // Switch root state
            context.CurrentState = newState;

            // Enter the current sub state of the root state
            if (currentSubState != null)
                currentSubState.EnterState();
        }
        else if (currentSuperState != null)  // Has this state a super state?
            currentSuperState.SetSubState(newState); // Set the substate of the parent to newState
    }

    protected void SetSuperState(PlayerState newSuperState) 
    {
        currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerState newSubState) 
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
