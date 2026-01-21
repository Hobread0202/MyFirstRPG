public class EnemyDeadState : IState<EnemyCtrl>
{
    public void Enter(EnemyCtrl enemy)
    {
        enemy.Anima.SetTrigger("Dead");
    }

    public void Execute(EnemyCtrl enemy)
    {
        //나중에 사라지는 셰이더값이 변동이라면 여기서 호출
        //셰이더끝나면 ChangeState(_enemyIdleState);
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
