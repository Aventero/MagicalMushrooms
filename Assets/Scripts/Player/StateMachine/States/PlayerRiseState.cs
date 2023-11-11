using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRiseState : PlayerState, IRootState
{
    private Vector3 targetPosition;

    public PlayerRiseState(PlayerStateMachine context, PlayerStateFactory factory, string name)
        : base(context, factory, name)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        // Switch to FallState once the upward momentum is gone.
        if (context.CurrentMovementY <= 0.0f || !context.IsJumpPressed)
        {
            SwitchState(factory.Fall());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Enter Rise");
        InitializeSubState();
        HandleJump();
    }

    public override void ExitState()
    {
        Debug.Log("Exit Rise");
    }

    public override void InitializeSubState()
    {
        if (!context.IsMovementPressed && !context.IsSneakPressed)
            SetSubState(factory.Idle());
        else if (context.IsMovementPressed && !context.IsSneakPressed)
            SetSubState(factory.Walk());
        else
            SetSubState(factory.Sneak());
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
        SimulateInertia();
    }

    private void HandleJump()
    {
        context.CurrentMovementY = context.InitialJumpVelocity;
        context.AppliedMovementY = context.InitialJumpVelocity;
    }

    public void HandleGravity()
    {
        // Gravity until the apex is reached
        float previousYVelocity = context.CurrentMovementY;
        context.CurrentMovementY += context.Gravity * Time.deltaTime;
        context.AppliedMovementY = (previousYVelocity + context.CurrentMovementY) * 0.5f; // Average the velocity
    }

    private void SimulateInertia()
    {

        if (context.CurrentMovementY > 0)
        {
            // Set the target position slightly below the current position
            targetPosition = context.heldObject.localPosition + Vector3.down * context.YInertiaStrength * 0.05f;
        }

        // Lerp the player's position towards the target position
        context.heldObject.localPosition = Vector3.Lerp(context.heldObject.localPosition, targetPosition, context.ObjectLerpSpeed * Time.deltaTime * 0.5f);

    }
}
