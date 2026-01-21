using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyType _enemyType;
    [SerializeField] int _maxCount = 1;      //최대 유지수
    [SerializeField] float _timer = 3.0f; //체크 주기



    IEnumerator Start()
    {
        //게임이 실행되는 동안 무한 반복
        while (true)
        {
            //현재 필드에 있는 몬스터 확인 (해당타입)
            int currentActive = EnemyPoolManager.Instance.GetActiveCount(_enemyType);

            // 필드몬스터가 최대유지수보다 적으면 소환
            if (currentActive < _maxCount)
            {
                Spawn();
            }

            //타이머시간만큼 기다렸다 진행
            yield return new WaitForSeconds(_timer);
        }
    }

    void Spawn()
    {
        Vector3 Pos = gameObject.transform.position;
        EnemyPoolManager.Instance.SpawnEnemy(_enemyType, Pos, Quaternion.identity, gameObject.transform);
    }
}