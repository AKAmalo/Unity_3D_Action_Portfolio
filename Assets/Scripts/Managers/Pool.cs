using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class Pool
{
    // 생성된 오브젝트를 저장하는 Queue
    private Queue<PoolObject> poolQueue = new Queue<PoolObject>();

    // 원본 프리팹
    private PoolObject prefab;

    // 부모 Transform
    private Transform parent;

    // 생성자
    public Pool(PoolObject prefab, int count, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        // 처음에 count개 생성
        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    // 새 오브젝트 생성
    private void CreateObject()
    {
        PoolObject obj = GameObject.Instantiate(prefab, parent);
        obj.OwnerPool = this;
        obj.gameObject.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    // pool에서 가져오기
    public PoolObject GetObject()
    {
        if(poolQueue.Count == 0)
        {
            CreateObject();
        }

        PoolObject obj = poolQueue.Dequeue();
        obj.gameObject.SetActive(true);

        // Pool에서 꺼낼 때 ParticleSystem을 다시 재생
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();

        if (particle != null)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Clear();
            particle.Play();
        }

        return obj;
    }

    // Pool로 반환
    public void ReturnObject(PoolObject obj)
    {
        obj.gameObject.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
