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
        // Debug.Log("Idle");
    }

    public void Update()
    {
        if(player.HasMoveInput())
        {
            player.ChangeState(new MoveState(player));
        }

        if (player.ConsumeJump() && player.IsGrounded())
        {
            player.Jump(); // 기존 점프 재사용
        }
    }

    public void Exit()
    {

    }
}