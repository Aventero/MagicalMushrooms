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
        //Debug.Log("WalkState");
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

        CheckSwitchStates();
    }
}
