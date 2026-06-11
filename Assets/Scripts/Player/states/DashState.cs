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

        rb.velocity = velocity;

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

        Vector3 velocity = rb.velocity;
        velocity.y = 0f;    // Dsah 종료 시 수직 속도 제거
        rb.velocity = velocity;
    }
}
