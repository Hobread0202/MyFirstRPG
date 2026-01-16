
public class EnemyMoveState : IState<EnemyCtrl>
{    
    public void Enter(EnemyCtrl enemy)
    {
        
    }

    public void Execute(EnemyCtrl enemy)
    {
        enemy.Nav.SetDestination(enemy.Target.position);
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
