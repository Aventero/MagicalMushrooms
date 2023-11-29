using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerState
{

    public PlayerWalkState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base(context, playerStateFactory, name) { }

    public override void CheckSwitchStates()
    {
        if (!context.IsMovementPressed)
            SwitchState(factory.Idle());
        else if (context.IsMovementPressed && context.IsSneakPressed)
            SwitchState(factory.Sneak());
    }

    public override void EnterState()
    {
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
        Vector3 movement = (context.transform.right * context.CurrentMovementInput.x + context.transform.forward * context.CurrentMovementInput.y) * context.WalkingSpeed;
        context.AppliedMovementX = movement.x;
        context.AppliedMovementZ = movement.z;

        // Only apply wobbling when grounded!
        if (context.CharacterController.isGrounded)
        {
            HandleCameraWobble();
            HandleHeldObjectWobble();
        }

        CheckSwitchStates();
    }

    void HandleCameraWobble()
    {

        // Camera wobble and Object wobble are the same Frequency, making it feel more "correct"
        float wobblePitch = Mathf.Sin(Time.time * context.wobbleFrequency) * context.cameraWobbleAmount; // Up - down
        float wobbleYaw = Mathf.Sin(Time.time * context.wobbleFrequency * 2f) * context.cameraWobbleAmount * 0.5f; // Left -Right

        // Apply wobble to camera's rotation
        Quaternion targetRotation = context.InitialCameraRotation * Quaternion.Euler(wobblePitch, wobbleYaw, 0f);
        Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, targetRotation, Time.deltaTime * context.ObjectLerpSpeed);
    }


    void HandleHeldObjectWobble()
    {
        // Calculate wobble amount -> sin(oscilation) * amplitude
        float wobbleX = Mathf.Sin(Time.time * context.wobbleFrequency) * context.walkWobbleAmount; // For side-to-side wobble
        float wobbleY = Mathf.Sin(Time.time * context.wobbleFrequency * 2f) * context.walkWobbleAmount * 0.5f; // For up-and-down wobble

        // Apply wobble to object's position
        Vector3 targetPosition = new Vector3(context.InitialHeldObjectPosition.x + wobbleX, context.InitialHeldObjectPosition.y + wobbleY, context.InitialHeldObjectPosition.z);
        context.heldObject.localPosition = Vector3.Lerp(context.heldObject.localPosition, targetPosition, Time.deltaTime * context.ObjectLerpSpeed);
   
    }
}
