using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    // Inspector에서 등록할 pool 정보
    [System.Serializable]
    public class  PoolData
    {
        public string key;  // Pool 이름
        public PoolObject prefab;   // 프리팹
        public int count = 10;  // 초기 생성 개수
    }

    // Inspector에서 여러 pool 등록
    [Header("Pool List")]
    [SerializeField]
    private List<PoolData> poolList =
        new List<PoolData>();

    // 실제 Pool 저장
    private Dictionary<string, Pool> pools =
        new Dictionary<string, Pool>();

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

        // Inspector에서 등록한 Pool 생성
        foreach (PoolData data in poolList)
        {
            Pool pool =
                new Pool(data.prefab, data.count, transform);

            pools.Add(data.key, pool);
        }
    }

    // Spawn
    public PoolObject Spawn(string key)
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogError($"Pool {key} 없음");
            return null;
        }

        return pools[key].GetObject();
    }
}
