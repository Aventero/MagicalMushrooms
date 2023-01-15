using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState, IRootState
{
    public PlayerJumpState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base (context, playerStateFactory, name) 
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
        //Debug.Log("JumpState");
        InitializeSubState(); 
        HandleJump();
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

    private void HandleJump()
    {
        context.CurrentMovementY = context.InitialJumpVelocity;
        context.AppliedMovementY = context.InitialJumpVelocity;
    }

    public void HandleGravity()
    {
        // Increase the Falling speed when at apex or the player has released the jump button
        bool isFalling = context.CurrentMovementY <= 0.0f || !context.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            // When the player is falling, increase his falling velocity!
            float previousYVelocity = context.CurrentMovementY;

            // Save for the next frame
            context.CurrentMovementY += context.Gravity * fallMultiplier * Time.deltaTime; // Save for the next frame

            // This is applied on the actual move ( The Average of the previous and current frame) -> Velocity Verlet Integration - Always same Jump.
            context.AppliedMovementY = Mathf.Max((previousYVelocity + context.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            // Gravity, when the player has not reached the jumping apex yet!
            float previousYVelocity = context.CurrentMovementY;
            context.CurrentMovementY += context.Gravity * Time.deltaTime;
            context.AppliedMovementY = (previousYVelocity + context.CurrentMovementY) * 0.5f;    // Average the velocity
        }
    }
}
