using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base (context, playerStateFactory, name) { }

    public override void CheckSwitchStates()
    {
        if (!context.IsMovementPressed)
            SwitchState(factory.Idle());
        else if (context.IsMovementPressed && !context.IsRunPressed)
            SwitchState(factory.Walk());
    }

    public override void EnterState()
    {
        Debug.Log("Entered: RunState");
        context.CurrentSpeed = context.RunningSpeed;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        // Set the movement based on the look direction
        Vector3 movement = (context.transform.right * context.CurrentMovementInput.x + context.transform.forward * context.CurrentMovementInput.y) * context.RunningSpeed;
        context.AppliedMovementX = movement.x;
        context.AppliedMovementZ = movement.z;

        CheckSwitchStates();
    }
}
