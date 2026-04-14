using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandState : IPlayerState
{
    private PlayerMovement player;
    private float timer;

    private float landDuration;
    private bool hardLanding;

    public LandState(PlayerMovement player, float duration, bool hardLanding)
    {
        this.player = player;
        this.landDuration = duration;
        this.hardLanding = hardLanding;
    }

    public void Enter()
    {
        timer = 0f;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.PlayLandingImpact(hardLanding);
        }

        if (hardLanding)
        {
            Debug.Log("LandState Enter -> HARD");
            player.SetCanRotate(false); // HardLanding 카메라 회전 금지
            player.SetRotateSpeed(80f);
            player.SetMoveSpeedMultiplier(0.35f);
            player.ConsumeJumpBuffer();

            // 모션 중 이동 제거(강제 정지)
            Rigidbody rb = player.GetRigidbody();
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
        else
        {
            Debug.Log("LandState Enter -> SOFT");
            player.SetCanRotate(true); // SoftLanding 카메라 회전 허용
            player.SetRotateSpeed(360f);
            player.SetMoveSpeedMultiplier(0.7f);
        }
    }

    public void Update()
    {
        if (!hardLanding)
        {
            if (player.CanJump())
            {
                player.ConsumeJumpBuffer();
                player.ChangeState(new JumpState(player));
                return;
            }
        }

        timer += Time.deltaTime;

        float startMultiplier = hardLanding ? 0.35f : 0.7f;
        float normalized = Mathf.Clamp01(timer / landDuration);
        float currentMultiplier = Mathf.Lerp(startMultiplier, 1f, normalized);

        player.SetMoveSpeedMultiplier(currentMultiplier);

        if (hardLanding)
        {
            Rigidbody rb = player.GetRigidbody();
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        // 일정 시간 후 전환 (착지 후 바로 이동 가능)
        if (timer >= landDuration)
        {
            if (player.HasMoveInput())
                player.ChangeState(new MoveState(player));
            else
                player.ChangeState(new IdleState(player));
        }
    }

    public void Exit()
    {
        player.SetCanRotate(true);
        player.SetRotateSpeed(360f);
        player.SetMoveSpeedMultiplier(1f);
    }
}