using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : IPlayerState
{
    private PlayerMovement player;
    private Rigidbody rb;

    private float dashDuration = 0.35f; // 대시 지속 시간
    private float timer;

    private Vector3 dashDirection;

    public DashState(PlayerMovement player)
    {
        this.player = player;
        this.rb = player.GetRigidbody();
    }

    public void Enter()
    {
        timer = 0f;

        player.StartDashCooldown();

        Vector2 input = player.GetMoveInput();

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        dashDirection = forward * input.y + right * input.x;

        if(dashDirection == Vector3.zero)
        {
            dashDirection = player.GetModelRoot().forward;
        }

        dashDirection.Normalize();

        rb.velocity = Vector3.zero; // 대시 시작 시 기존 속도 제거

        player.SetCanRotate(false); // 대시 중 회전 방지

        player.GetAnimator().SetTrigger("Dash");

        // Dash 이벤트 발생
        GameEvent.OnPlayerDash?.Invoke(
            player.GetTransform().position,
            player.GetTransform().rotation);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        Vector3 moveDir = dashDirection;

        if(player.TryGetGroundNormal(out Vector3 groundNormal))
        {
            moveDir = Vector3.ProjectOnPlane(dashDirection, groundNormal).normalized;
        }

        Vector3 velocity = moveDir * player.DashSpeed;

        // 공중에서 기존 중력 속도 유지
        if (!player.IsGrounded())
        {
            velocity.y = rb.velocity.y;
        }
        else
        {
            // 지면에서는 Dash가 수직속도를 만들지 못하게 함
            velocity.y = Mathf.Min(rb.velocity.y, 0f);
        }

            rb.velocity = velocity;

        // 경사면 기울기 유지
        player.AlignToGround();

        // 대쉬 종료 후 공중이면 Fall
        if (!player.IsGrounded())
        {
            player.ChangeState(new FallState(player));
            return;
        }

        if (timer >= dashDuration)
        {
            if(player.HasMoveInput())
            {
                player.ChangeState(new MoveState(player));
            }
            else
            {
                player.ChangeState(new IdleState(player));
            }
        }
    }

    public void Exit()
    {
        player.SetCanRotate(true); // 대시 종료 후 회전 허용
    }
}