using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerData",
    menuName = "Game Data/Player Data")]

public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Jump")]
    public float jumpForce = 5f;

    // 코요테 타임
    public float coyoteTime = 0.1f;

    // 점프 버퍼
    public float jumpBufferTime = 0.1f;

    [Header("Fall")]
    // 공중 판정 유예
    public float fallGraceTime = 0.12f;

    // Hard Landing 기준
    public float hardLandingThreshold = -7f;

    [Header("Dash")]
    public float dashSpeed = 15f;

    // 대쉬 쿨다운
    public float dashCooldown = 1f;
}
