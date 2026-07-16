using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 사용하는 이펙트를 관리하는 클래스
/// Event를 받아 ObjectPool에서 이펙트를 생성한다.
/// </summary>
public class EffectManager : MonoBehaviour
{
    //Singlelton
    public static EffectManager Instance;

    [SerializeField] private Transform player;

    private void Awake()
    {
        Instance = this;
    }

    // Event 등록
    private void OnEnable()
    {
        GameEvent.OnPlayerJump += PlayJumpDust;
        GameEvent.OnPlayerLand += PlayLandDust;
        GameEvent.OnPlayerDash += PlayDashSmoke;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerJump -= PlayJumpDust;
        GameEvent.OnPlayerLand -= PlayLandDust;
        GameEvent.OnPlayerDash -= PlayDashSmoke;
    }

    /// <summary>
    /// 플레이어 점프 시 먼지 생성
    /// </summary>
    private void PlayJumpDust(Vector3 position, Quaternion rotation)
    {
        if (player == null)
        {
            Debug.LogWarning("Player가 연결되지 않았습니다.");
            return;
        }

        SpawnEffect(
                "JumpDust",
                position + Vector3.down * 0.9f,
                Quaternion.identity);
    }

    /// <summary>
    /// 플레이어 착지 시 먼지 생성
    /// </summary>
    private void PlayLandDust(Vector3 position, Quaternion rotation, bool hardLanding)
    {
        // Inspector에 Player가 연결되지 않은 경우
        if (player == null)
        {
            Debug.LogWarning("Player가 연결되지 않았습니다.");
            return;
        }

        if(hardLanding)
        {
            SpawnEffect(
                "HardLandDust",
                position + Vector3.down * 0.9f,
                Quaternion.identity);
        }
        else
        {
            SpawnEffect(
                "SoftLandDust",
                position + Vector3.down * 0.9f,
                Quaternion.identity);
        }
    }

    ///<summary>
    /// Dash Smoke 생성
    ///</summary>

    private void PlayDashSmoke(Vector3 position, Quaternion rotation)
    {
        if (player == null)
        {
            Debug.LogWarning("Player가 연결되지 않았습니다.");
            return;
        }
        // 플레이어가 바라보는 방향으로 생성
        SpawnEffect(
            "DashSmoke",
            position + Vector3.down * 0.9f,
            rotation);
    }

    /// <summary>
    /// Pool에서 이펙트를 꺼내 원하는 위치와 회전에 생성하는 공통 함수
    /// 앞으로 모든 이펙트는 이 함수만 호출하면 됨.
    /// </summary>
    public void SpawnEffect(string key, Vector3 position, Quaternion rotation)
    {
        PoolObject effect = PoolManager.Instance.Spawn(key);

        if (effect == null)
            return;

        effect.transform.position = position;
        effect.transform.rotation = rotation;
    }
}