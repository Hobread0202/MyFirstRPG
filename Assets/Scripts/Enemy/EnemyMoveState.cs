
public class EnemyMoveState : IState<EnemyCtrl>
{    

    public void Enter(EnemyCtrl enemy)
    {
        
    }

    public void Execute(EnemyCtrl enemy)
    {
        if (enemy.Nav.gameObject.activeInHierarchy && enemy.Nav.enabled && enemy.Nav.isOnNavMesh)
        {
            if (enemy.Target != null)
            {
                enemy.Nav.SetDestination(enemy.Target.position);
            }
        }
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
