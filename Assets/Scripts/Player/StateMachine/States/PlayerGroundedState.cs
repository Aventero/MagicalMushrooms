using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base (context, playerStateFactory, name) 
    {
        isRootState = true;
    }


    public override void CheckSwitchStates()
    {
        if (context.IsJumpPressed)
            SwitchState(factory.Jump());
        else if (!context.CharacterController.isGrounded)
            SwitchState(factory.Fall());
    }

    public override void EnterState()
    {
        Debug.Log("Entered: GroundedState");
        InitializeSubState(); 
        HandleGravity();
    }

    public override void ExitState()
    {
    }

    public void HandleGravity()
    {
        context.CurrentMovementY = context.Gravity * 0.1f;
        context.AppliedMovementY = context.Gravity * 0.1f;
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
        CheckSwitchStates();
    }
}
