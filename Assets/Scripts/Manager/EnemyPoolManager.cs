using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyType { Duzi }  //타입정의
public class EnemyPoolManager : MonoBehaviour
{
    [Serializable] public class EnemyPrefabData
    {
        public EnemyType type;
        public EnemyCtrl prefab;
        public int capacity;    //처음 만들어둘 수량
        public int maxSize;     //최대 수량
    }

    //싱글톤
    public static EnemyPoolManager Instance;

    [SerializeField] EnemyPrefabData[] _enemyPrefab; //프리팹들
    [SerializeField] Transform _poolPos;    //비활성화시켜둘위치    
    [SerializeField] PlayerCtrl _player;    //추적시킬 플레이어

    // 타입별 풀 저장
    private Dictionary<EnemyType, ObjectPool<EnemyCtrl>> enemyPools;

    private void Awake()
    {
        //다른매니저가 존재하는데 내가 아니라면
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //이객체를 싱글톤 지정
        Instance = this;

        InitializePool();
    }

    void InitializePool()
    {
        enemyPools = new Dictionary<EnemyType, ObjectPool<EnemyCtrl>>();

        //프리팹들 초기설정
        foreach (var data in _enemyPrefab)
        {
            var pool = new ObjectPool<EnemyCtrl>
            (
                //만들 프리팹
                createFunc: () => CreateEnemy(data.prefab, data.type),

                //꺼낼때 실행
                actionOnGet: (enemyCtrl) =>
                {
                    enemyCtrl.gameObject.SetActive(true);
                    enemyCtrl.OnSpawnFromPool();
                },

                //넣을때 실행
                actionOnRelease: (enemyCtrl) =>
                {
                    enemyCtrl.OnReturnToPool();
                    enemyCtrl.gameObject.SetActive(false);
                },

                //파괴될때 실행
                actionOnDestroy: (enemyCtrl) => Destroy(enemyCtrl.gameObject),

                //생성시켜둘 수량
                defaultCapacity: data.capacity,

                //최대수량
                maxSize: data.maxSize
            );

            enemyPools.Add(data.type, pool);
        }

        EnemyCtrl CreateEnemy(EnemyCtrl prefab, EnemyType type)
        {
            EnemyCtrl enemy = Instantiate(prefab, _poolPos);
            enemy.gameObject.SetActive(false);
            enemy.SetEnemyType(type);
            enemy.SetPoolManager(this);
            return enemy;
        }


    }

    public EnemyCtrl SpawnEnemy(EnemyType type, Vector3 position, Quaternion rotation, Transform mySpawnArea)
    {
        EnemyCtrl enemy = enemyPools[type].Get();
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;

        enemy.SetTarget(_player.PlayerTransform);   //플레이어위치값
        enemy.SetSpawnArea(mySpawnArea);

        return enemy;
    }

    public void ReturnEnemy(EnemyType type, EnemyCtrl enemy)
    {
        enemyPools[type].Release(enemy);
    }

    public int GetActiveCount(EnemyType type)
    {
        return enemyPools[type].CountActive;
    }
}
