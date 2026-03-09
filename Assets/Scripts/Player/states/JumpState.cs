using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IPlayerState
{
    private PlayerMovement player;
    private Rigidbody rb;

    public JumpState(PlayerMovement player)
    {
        this.player = player;
        this.rb = player.GetRigidbody();
    }

    public void Enter()
    {
        // 점프 실행
        player.Jump();

        // 애니메이션 트리거
        player.GetAnimator().SetTrigger("Jump");
    }

    public void Update()
    {
        // 공중 이동 (MoveState와 동일하지만 약하게)
        AirMove();

        // 최고점 도달 -> 낙하 상태 전환
        if (rb.velocity.y <= 0f)
        {
            player.ChangeState(new FallState(player));
        }
    }

    public void Exit()
    {

    }

    private void AirMove()
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

        // 공중에서는 영향 감소
        float airControl = 0.3f;

        velocity.x = Mathf.Lerp(velocity.x, moveDir.x * player.MoveSpeed, airControl);
        velocity.z = Mathf.Lerp(velocity.z, moveDir.z * player.MoveSpeed,airControl);

        rb.velocity = velocity;

        player.Rotate(moveDir);
    }
}
