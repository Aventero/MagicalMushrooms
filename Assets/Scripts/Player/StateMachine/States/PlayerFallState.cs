
using TMPro;
using UnityEngine;

public class PlayerFallState : PlayerState, IRootState
{
    private Vector3 targetPosition;  // The position we are lerping to

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
        Debug.Log("Enter Fall");
        InitializeSubState();
    }

    public override void ExitState()
    {
        Debug.Log("Exit Fall");
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

    public void HandleGravity()
    {
        float previousYVelocity = context.CurrentMovementY;
        context.CurrentMovementY += context.Gravity * 2.0f * Time.deltaTime; // Higher multiplier to simulate faster fall
        context.AppliedMovementY = Mathf.Max((previousYVelocity + context.CurrentMovementY) * 0.5f, -20.0f); // Average the velocity
    }

    private void SimulateInertia()
    {

        if (context.CurrentMovementY < 0) // If the player is falling
        {
            // Set the target position slightly below the current position
            targetPosition = context.heldObject.localPosition + Vector3.up * context.YInertiaStrength * 0.05f;
        }

        // Lerp the player's position towards the target position
        context.heldObject.localPosition = Vector3.Lerp(context.heldObject.localPosition, targetPosition, context.ObjectLerpSpeed * Time.deltaTime);
    }
}
