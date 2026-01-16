
using UnityEngine;

public class IdleState : IState<PlayerCtrl>
{
    public void Enter(PlayerCtrl player)
    {
        player.Anima.SetFloat("MoveY", 0f);
    }
    public void Execute(PlayerCtrl player)
    {

        if (player.MoveInput.sqrMagnitude > 0)
        {
            player.ChangeState(player.MoveState);
        }
    }

    public void Exit(PlayerCtrl player)
    {
        
    }
}
