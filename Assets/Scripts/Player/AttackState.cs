public class AttackState : IState<PlayerCtrl>
{
    public void Enter(PlayerCtrl player)
    {
        player.Anima.applyRootMotion = true;
        player.Anima.SetTrigger("Attack");
    }

    public void Execute(PlayerCtrl player)
    {
        
    }

    public void Exit(PlayerCtrl player)
    {
        player.Anima.applyRootMotion = false;
        
    }


}
