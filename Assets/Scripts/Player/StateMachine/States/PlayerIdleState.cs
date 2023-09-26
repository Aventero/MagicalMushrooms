using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base (context, playerStateFactory, name) { }

    public override void CheckSwitchStates()
    {
        if (context.IsMovementPressed && context.IsSneakPressed)
            SwitchState(factory.Sneak());
        else if (context.IsMovementPressed)
            SwitchState(factory.Walk());
    }

    public override void EnterState()
    {
        //Debug.Log("IdleState");
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
        CameraWobble();
        HeldObjectWobble();
    }

    private void CameraWobble()
    {
        // Not moving -> Camera initial rotation
        Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, context.InitialCameraRotation, Time.deltaTime * context.ObjectLerpSpeed);
    }

    private void HeldObjectWobble()
    {
        // Return object to initial position
        context.heldObject.localPosition = Vector3.Lerp(context.heldObject.localPosition, context.InitialHeldObjectPosition, Time.deltaTime * context.ObjectLerpSpeed);
        context.heldObject.localRotation = Quaternion.Slerp(context.heldObject.localRotation, context.InitialHeldObjectRotation, Time.deltaTime * context.ObjectLerpSpeed);
    }
}
