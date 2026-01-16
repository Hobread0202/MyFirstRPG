public class EnemyIdleState : IState<EnemyCtrl>
{
    public void Enter(EnemyCtrl enemy)
    {
        
    }

    public void Execute(EnemyCtrl enemy)
    {
        enemy.Nav.SetDestination(enemy.SpawnArea.position);
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
