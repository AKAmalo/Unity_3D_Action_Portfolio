using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent
{
    // 플레이어 점프
    public static Action OnPlayerJump;

    // 플레이어 착지
    public static Action OnPlayerLand;

    // 플레이어 대쉬
    public static Action OnPlayerDash;
}
