using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    // 어띠 Pool에서 관리되는지 저장
    public Pool OwnerPool { get; set; }

    // Pool로 되돌아가기
    public void ReturnToPool()
    {
        if (OwnerPool != null)
        {
            OwnerPool.ReturnObject(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
