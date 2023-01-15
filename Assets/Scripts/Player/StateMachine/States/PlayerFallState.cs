
using UnityEngine;

public class PlayerFallState : PlayerState, IRootState
{

    public PlayerFallState(PlayerStateMachine context, PlayerStateFactory factory, string name) 
        : base(context, factory, name)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (context.CharacterController.isGrounded)
            SwitchState(factory.Grounded());
    }

    public override void EnterState()
    {
        Debug.Log("Enter: FallState");
        InitializeSubState();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if (!context.IsMovementPressed && !context.IsRunPressed)
            SetSubState(factory.Idle());
        else if (context.IsMovementPressed && !context.IsRunPressed)
            SetSubState(factory.Walk());
        else
            SetSubState(factory.Run());
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public void HandleGravity()
    {
        float previousYVelocity = context.CurrentMovementY;
        context.CurrentMovementY += context.Gravity * Time.deltaTime;
        context.AppliedMovementY = Mathf.Max((previousYVelocity + context.CurrentMovementY) * 0.5f, -20f);
    }
}
