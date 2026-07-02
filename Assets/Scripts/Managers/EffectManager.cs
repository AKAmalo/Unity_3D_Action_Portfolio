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
        GameEvent.OnPlayerLand += PlayLandDust;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerLand -= PlayLandDust;
    }

    // 착지 먼지

    /// <summary>
    /// 플레이어 착지 시 먼지 생성
    /// </summary>
    private void PlayLandDust()
    {
        // Inspector에 Player가 연결되지 않은 경우
        if (player == null)
        {
            Debug.LogWarning("Player가 연결되지 않았습니다.");
            return;
        }

        PoolObject dust =
            PoolManager.Instance.Spawn("LandDust");

        if (dust == null)
            return;

        // 바닥에 붙도록 살짝 내림
        dust.transform.position = player.position + Vector3.down * 0.9f;

        dust.transform.rotation = Quaternion.identity;
    }
}
