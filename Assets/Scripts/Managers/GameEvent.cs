using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent
{
    // 위치와 회전을 함께 전달
    // 플레이어 점프
    public static Action<Vector3, Quaternion> OnPlayerJump;

    // 플레이어 착지  true = Hard, false = Soft
    public static Action<Vector3, Quaternion, bool> OnPlayerLand;

    // 플레이어 대쉬
    public static Action<Vector3, Quaternion> OnPlayerDash;
}
