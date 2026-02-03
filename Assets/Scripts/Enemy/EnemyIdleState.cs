public class EnemyIdleState : IState<EnemyCtrl>
{
    public void Enter(EnemyCtrl enemy)
    {
        //idle모션으로 전환
        enemy.EnableAllColliders();
    }

    public void Execute(EnemyCtrl enemy)
    {
        //네비메쉬 활성화체크
        if (enemy.Nav.gameObject.activeInHierarchy && enemy.Nav.enabled && enemy.Nav.isOnNavMesh)
        {
            //타겟체크
            if (enemy.Target != null && enemy.TargetInDetectionArea())
            {
                //EnemyMoveState에서 타겟따라가게하고
                enemy.ChangeState(enemy.EnemyMoveState);
            }
            else
            {
                //타겟이 없으면 스폰 위치로 이동
                enemy.Nav.SetDestination(enemy.SpawnArea.position);
            }
        }
    }

    public void Exit(EnemyCtrl enemy)
    {
        
    }
}
