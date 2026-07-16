using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <Audio 재생이 끝나면 자동으로 Pool 반환
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

    private void OnEnable()
    {
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        if(audioSource.clip == null)
        {
            Debug.LogWarning("AudioSource has no clip assigned.");
            yield break;
        }

        yield return new WaitForSeconds(audioSource.clip.length);
        poolObject.ReturnToPool();
    }
}
