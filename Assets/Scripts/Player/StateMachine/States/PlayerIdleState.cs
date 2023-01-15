using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base (context, playerStateFactory, name) { }

    public override void CheckSwitchStates()
    {
        if (context.IsMovementPressed && context.IsRunPressed)
            SwitchState(factory.Run());
        else if (context.IsMovementPressed)
            SwitchState(factory.Walk());
    }

    public override void EnterState()
    {
        Debug.Log("Entered: IdleState");
        context.AppliedMovementX = 0;
        context.AppliedMovementZ = 0; 
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
