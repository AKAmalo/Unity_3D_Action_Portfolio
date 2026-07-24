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
    [SerializeField] private AudioClip footstepClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        GameEvent.OnPlayerJump += PlayJumpSound;
        GameEvent.OnPlayerLand += PlayLandSound;
        GameEvent.OnPlayerDash += PlayDashSound;
        GameEvent.OnPlayerFootstep += PlayFootstepSound;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerJump -= PlayJumpSound;
        GameEvent.OnPlayerLand -= PlayLandSound;
        GameEvent.OnPlayerDash -= PlayDashSound;
        GameEvent.OnPlayerFootstep -= PlayFootstepSound;
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

    private void PlayFootstepSound(Vector3 position)
    {
        SpawnSound(footstepClip, position);
    }

    public void SpawnSound(AudioClip clip, Vector3 position)
    {
        // 재생할 AudioClip이 없는 경우
       if (clip == null)
        {
            Debug.LogWarning("SoundManager: 재생하려는 AudioClip이 없습니다.");
            return;
        }

       // Sound Pool에서 오브젝트 가져오기
        PoolObject sound = PoolManager.Instance.Spawn("Sound");

        if (sound == null)
        {
            Debug.LogWarning("SoundManager: Sound Pool에서 오브젝트를 가져오지 못했습니다.");
            return;
        }

        // 사운드 재생 위치 설정
        sound.transform.position = position;

        // PoolAudioAutoReturn 가져오기
        PoolAudioAutoReturn autoReturn = sound.GetComponent<PoolAudioAutoReturn>();

        if (autoReturn == null)
        {
            Debug.LogWarning("Sound 오브젝트에 PoolAudioAutoReturn 컴포넌트가 없습니다.");

            sound.ReturnToPool();
            return;
        }

        // AudipClip 설정 + 재생
        // 재생이 끝나면 자동으로 Pool 반환
        autoReturn.Play(clip); 
    }
}