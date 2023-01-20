using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState, IRootState
{
    private float fakeGroundedTimer = 0;
    private const float maxHelpSeconds = 0.25f;
    private bool isGrounded = true;

    public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base (context, playerStateFactory, name) 
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (context.IsJumpPressed && isGrounded)
            SwitchState(factory.Jump());
        else if (!context.CharacterController.isGrounded && !isGrounded)
            SwitchState(factory.Fall());
    }

    public override void EnterState()
    {
        fakeGroundedTimer = 0;
        InitializeSubState(); 
        HandleGravity();
    }

    public override void ExitState()
    {
        fakeGroundedTimer = 0;
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
        SetGroundedState();
        CheckSwitchStates();
    }

    private void SetGroundedState()
    {
        if (!context.CharacterController.isGrounded)
        {
            // Player is not grounded
            fakeGroundedTimer += Time.deltaTime;

            if (fakeGroundedTimer >= maxHelpSeconds)
            {
                // Player is not allowed to jump!
                isGrounded = false;
                return;
            }
        }
        else
            fakeGroundedTimer = 0; // Reset the timer

        // The Player is still grounded
        isGrounded = true;
    }
}
