
using UnityEngine;

public class IdleState : IState<PlayerCtrl>
{
    float _currentMoveY; //애니메이션 파라미터값
    public void Enter(PlayerCtrl player)
    {
        //진행중이던 파라미터값 받아오기
        _currentMoveY = player.Anima.GetFloat("MoveY");
    }
    public void Execute(PlayerCtrl player)
    {
        _currentMoveY = Mathf.Lerp(_currentMoveY, 0f, Time.deltaTime * 5f);        
        player.Anima.SetFloat("MoveY", _currentMoveY);


        if (player.MoveInput.sqrMagnitude > 0)
        {
            player.ChangeState(player.MoveState);
        }
    }

    public void Exit(PlayerCtrl player)
    {
        
    }
}
