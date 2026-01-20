public class EnemyDeadState : IState<EnemyCtrl>
{
    public void Enter(EnemyCtrl enemy)
    {
        enemy.Anima.SetTrigger("Dead");
    }

    public void Execute(EnemyCtrl enemy)
    {
        
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
