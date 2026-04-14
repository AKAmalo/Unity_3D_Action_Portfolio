using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IPlayerState
{
    private PlayerMovement player;
    private Rigidbody rb;

    public MoveState(PlayerMovement player)
    {
        this.player = player;
        this.rb = player.GetRigidbody();
    }

    public void Enter()
    {

    }

    public void Update()
    {
        Move();

        // Idle 전환
        if (!player.HasMoveInput())
        {
            player.ChangeState(new IdleState(player));
            return;
        }

        // Run 전환
        if(player.IsRunning())
        {
            player.ChangeState(new RunState(player));
            return;
        }

        if(player.CanJump())
        {
            player.ConsumeJumpBuffer();
            player.ChangeState(new JumpState(player));
            return;
        }

        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float normalizedSpeed = speed / player.RunSpeed;

        player.GetAnimator().SetFloat("Speed", normalizedSpeed);
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

        velocity.x = moveDir.x * player.MoveSpeed * speedMultiper;
        velocity.z = moveDir.z * player.MoveSpeed * speedMultiper;

        rb.velocity = velocity;

        player.Rotate(moveDir);
    }
}
