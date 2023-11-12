using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElevateState : PlayerState
{
    private float elevationSpeed = 2f; 

    public PlayerElevateState(PlayerStateMachine context, PlayerStateFactory playerStateFactory, string name)
        : base(context, playerStateFactory, name) 
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
       
    }

    public override void EnterState()
    {
        Debug.Log("Enter Elevate");
        InitializeSubState();
        context.CurrentMovementY = Mathf.Max(context.CurrentMovementY * 0.5f, -3);
        context.AppliedMovementY = Mathf.Max(context.AppliedMovementY * 0.5f, -3);
    }

    public override void ExitState()
    {
        Debug.Log("Exit Elevate");
        context.CurrentMovementY = context.CurrentMovementY * 0.5f;
        context.AppliedMovementY = context.AppliedMovementY * 0.5f;
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
        context.AppliedMovementY += elevationSpeed * Time.deltaTime;
    }
}
