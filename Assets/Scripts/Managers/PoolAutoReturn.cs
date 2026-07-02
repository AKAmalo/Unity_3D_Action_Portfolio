using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 파티클 재생이 끝나면 자동으로 ObjectPool에 반환
/// </summary>
public class PoolAutoReturn : MonoBehaviour
{
    private PoolObject poolObject;

    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
    }

    private void OnParticleSystemStopped()
    {
        poolObject.ReturnToPool();
    }
}
