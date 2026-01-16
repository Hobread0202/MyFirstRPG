
public class EnemyMoveState : IState<EnemyCtrl>
{
    EnemyCtrl _enemy;
    public void Enter(EnemyCtrl Parameter)
    {
        
    }

    public void Execute(EnemyCtrl Parameter)
    {
        _enemy.Nav.SetDestination(_enemy.Target.position);
    }

    public void Exit(EnemyCtrl Parameter)
    {
        
    }
}
