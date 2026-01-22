using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    EnemyCtrl _owner; // 부모에서 가져오기

    private void Awake()
    {
        _owner = GetComponentInParent<EnemyCtrl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerCtrl>(out PlayerCtrl player))
            {
                player.TakeDamage(_owner.EnemyData.Damage);
            }
        }
    }
}
