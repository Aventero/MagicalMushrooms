
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    PlayerStateMachine context;
    Dictionary<string, PlayerState> playerStates = new Dictionary<string, PlayerState>();

    public PlayerStateFactory(PlayerStateMachine context)
    {
        this.context = context;
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
        return playerStates["Idle"];
    }

    public PlayerState Walk()
    {
        return playerStates["Walk"];
    }

    public PlayerState Run()
    {
        return playerStates["Run"];
    }

    public PlayerState Jump()
    {
        return playerStates["Jump"];
    }

    public PlayerState Fall()
    {
        return playerStates["Fall"];
    }

    public PlayerState Grounded()
    {
        return playerStates["Grounded"];
    }
}
