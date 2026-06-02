using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IPlayerState
{
    private PlayerMovement player;
    private Rigidbody rb;
    private float maxFallSpeed;

    // 낙하 중력 배수
    private float fallGravityMultiplier = 1.3f;

    public FallState(PlayerMovement player)
    {
        this.player = player;
        this.rb = player.GetRigidbody();
    }

    public void Enter()
    {
        // 낙하 애니메이션 진입
        player.GetAnimator().SetBool("IsFalling", true);
        maxFallSpeed = 0f;
    }

    public void Update()
    {
        // Fall Gravity 강화
        if(rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime;
        }

        AirMove();

        // 낙하 최고 속도 기록
        if(rb.velocity.y < maxFallSpeed)
        {
            maxFallSpeed = rb.velocity.y;
        }

        // 착지
        if (player.IsGrounded() && rb.velocity.y < 0f)
        {
            Debug.Log("MaxFallSpeed: " +  maxFallSpeed);

            if (maxFallSpeed < player.hardLandingThreshold)
            {
                Debug.Log(">>> HARD LANDING BRANCH");
                // Hard Landing
                player.GetAnimator().SetTrigger("HardLand");
                player.ChangeState(new LandState(player, 0.8f, true));
            }
            else
            {
                Debug.Log(">>> SOFT LANDING BRANCH");
                // Soft Landing
                player.GetAnimator().SetTrigger("SoftLand");
                player.ChangeState(new LandState(player, 0.15f, false));
            }

            return;
        }
    }

    public void Exit()
    {
        player.GetAnimator().SetBool("IsFalling", false);
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

        if(player.OnSlope())
        {
            moveDir = Vector3.ProjectOnPlane(moveDir, player.GetSlopeNormal()).normalized;
        }

        Vector3 velocity = rb.velocity;

        float airControl = 0.3f;

        velocity.x = Mathf.Lerp(velocity.x, moveDir.x * player.MoveSpeed, airControl);
        velocity.z = Mathf.Lerp(velocity.z, moveDir.z * player.MoveSpeed, airControl);

        rb.velocity = velocity;

        player.Rotate(moveDir);
    }
}
