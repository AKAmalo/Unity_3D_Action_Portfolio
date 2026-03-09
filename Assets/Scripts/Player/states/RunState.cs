using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : IPlayerState
{
    private PlayerMovement player;
    private Rigidbody rb;

    public RunState(PlayerMovement player)
    {
        this.player = player;
        this.rb = player.GetRigidbody();
    }

    public void Enter()
    {
        // Run 애니메이션 진입
        player.GetAnimator().SetFloat("Speed", 1f);
    }

    public void Update()
    {
        Move();

        // Shift 떼면 Walk로
        if (!player.IsRunning())
        {
            player.ChangeState(new MoveState(player));
            return;
        }

        // 입력 없으면 Idle
        if (!player.HasMoveInput())
        {
            player.ChangeState(new IdleState(player));
            return;
        }

        // 점프
        if(player.ConsumeJump() && player.IsGrounded())
        {
            player.ChangeState(new JumpState(player));
            return;
        }
    }

    public void Exit()
    {

    }

    private void Move()
    {
        Vector2 input = player.GetMoveInput();

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.y + right * input.x;
        moveDir.Normalize();

        Vector3 velocity = rb.velocity;

        float speedMultiper = player.GetMoveSpeedMultiplier();

        velocity.x = moveDir.x * player.RunSpeed * speedMultiper;
        velocity.z = moveDir.z * player.RunSpeed * speedMultiper;

        rb.velocity = velocity;

        player.Rotate(moveDir);
    }
}
