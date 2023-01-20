using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    Dictionary<string, PlayerState> playerStates = new Dictionary<string, PlayerState>();

    public PlayerStateFactory(PlayerStateMachine context)
    {
        AddPlayerState(new PlayerIdleState(context, this, "Idle"));
        AddPlayerState(new PlayerWalkState(context, this, "Walk"));
        AddPlayerState(new PlayerRunState(context, this, "Run"));
        AddPlayerState(new PlayerJumpState(context, this, "Jump"));
        AddPlayerState(new PlayerFallState(context, this, "Fall"));
        AddPlayerState(new PlayerGroundedState(context, this, "Grounded"));
    }

    private void AddPlayerState(PlayerState playerState)
    {
        PlayerState state = playerState;
        playerStates.Add(state.Name, playerState);
    }

    public PlayerState Idle()
    {
        // Debug.Log("Idle");
        return playerStates["Idle"];
    }

    public PlayerState Walk()
    {
        // Debug.Log("Walk");
        return playerStates["Walk"];
    }

    public PlayerState Run()
    {
        // Debug.Log("Run");
        return playerStates["Run"];
    }

    public PlayerState Jump()
    {
        // Debug.Log("Jump");
        return playerStates["Jump"];
    }

    public PlayerState Fall()
    {
        // Debug.Log("Fall");
        return playerStates["Fall"];
    }

    public PlayerState Grounded()
    {
        // Debug.Log("Grounded");
        return playerStates["Grounded"];
    }
}
