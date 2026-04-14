using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerMovement player;

    public IdleState(PlayerMovement player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.GetAnimator().SetFloat("Speed", 0f);
    }

    public void Update()
    {
        if(player.HasMoveInput())
        {
            player.ChangeState(new MoveState(player));
            return;
        }

        if (player.CanJump())
        {
            player.ConsumeJumpBuffer();
            player.ChangeState(new JumpState(player));
            return;
        }
    }

    public void Exit()
    {

    }
}