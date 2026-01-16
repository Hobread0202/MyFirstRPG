using UnityEngine;

public class Detectionarea : MonoBehaviour
{
    EnemyCtrl _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<EnemyCtrl>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.ChangeState(_enemy.EnemyMoveState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.ChangeState(_enemy.EnemyIdleState);
        }
    }
}
