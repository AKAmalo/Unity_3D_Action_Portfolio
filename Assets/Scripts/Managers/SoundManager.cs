using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // Inspector에서 사운드 클립을 등록하고, PoolManager에서 해당 키로 오브젝트를 가져와 재생하는 방식으로 구현
    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip softLandClip;
    [SerializeField] private AudioClip hardLandClip;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvent.OnPlayerJump += PlayJumpSound;
        GameEvent.OnPlayerLand += PlayLandSound;
        GameEvent.OnPlayerDash += PlayDashSound;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerJump -= PlayJumpSound;
        GameEvent.OnPlayerLand -= PlayLandSound;
        GameEvent.OnPlayerDash -= PlayDashSound;
    }

    private void PlayJumpSound(Vector3 pos, Quaternion rot)
    {
        SpawnSound(jumpClip, pos);
    }

    private void PlayLandSound(Vector3 pos, Quaternion rot, bool hardLanding)
    {
        if (hardLanding)
        {
            SpawnSound(hardLandClip, pos);
        }
        else
        {
            SpawnSound(softLandClip, pos);
        }
    }

    private void PlayDashSound(Vector3 pos, Quaternion rot)
    {
        SpawnSound(dashClip, pos);
    }

    public void SpawnSound(AudioClip clip, Vector3 position)
    {
        PoolObject sound = PoolManager.Instance.Spawn("Sound");

        if (sound == null)
            return;

        sound.transform.position = position;

        AudioSource source = sound.GetComponent<AudioSource>();

        source.clip = clip;
        source.Play();
    }
}