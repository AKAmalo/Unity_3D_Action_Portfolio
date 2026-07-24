using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio 재생이 끝나면 자동으로 Pool 반환
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class PoolAudioAutoReturn : MonoBehaviour
{
    private PoolObject poolObject;
    private AudioSource audioSource;

    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        // 기존 실행 중인 ReturnRoutine을 중지
        StopAllCoroutines();
        audioSource.Stop();

        // 전달받은 AudioClip 설정
        audioSource.clip = clip;

        // Clip이 없는 경우 경고 출력 후 종료
        if (audioSource.clip == null)
        {
            Debug.LogWarning("PoolAudioAutoReturn: AudioClip이 설정되지 않았습니다.");

            // Clip이 없으면 Pool 반환
            poolObject.ReturnToPool();
            return;
        }

        // 실제 Audio 재생
        audioSource.Play();

        // Audio 재생이 끝난 후 pool 반환
        StartCoroutine(ReturnRoutine());
    }


    private IEnumerator ReturnRoutine()
    {
        // AudioClip 길이만큼 대기
        yield return new WaitForSeconds(audioSource.clip.length);

        // Audio 정지
        audioSource.Stop();

        // 다음 사용을 위해 Clip 초기화
        audioSource.clip = null;

        // Pool 반환
        poolObject.ReturnToPool();
    }
}
