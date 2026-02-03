
public class EnemyMoveState : IState<EnemyCtrl>
{    

    public void Enter(EnemyCtrl enemy)
    {
        enemy.EnableAllColliders();
    }

    public void Execute(EnemyCtrl enemy)
    {
        if (enemy.Nav.gameObject.activeInHierarchy && enemy.Nav.enabled && enemy.Nav.isOnNavMesh)
        {
            if (enemy.Target != null)
            {
                enemy.Nav.SetDestination(enemy.Target.position);
            }
            else
            {
                enemy.ChangeState(enemy.EnemyIdleState);
            }
        }
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
