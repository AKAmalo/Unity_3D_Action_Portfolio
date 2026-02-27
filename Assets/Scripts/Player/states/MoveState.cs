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
        this.rb = player.GetComponent<Rigidbody>();
    }

    public void Enter()
    {

    }

    public void Update()
    {
        Move();

        if(!player.HasMoveInput())
        {
            player.ChangeState(new IdleState(player));
        }

        if(player.ConsumeJump() && player.IsGrounded())
        {
            player.Jump();
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

        velocity.x = moveDir.x * player.MoveSpeed;
        velocity.z = moveDir.z * player.MoveSpeed;

        rb.velocity = velocity;

        player.Rotate(moveDir);
    }
}
