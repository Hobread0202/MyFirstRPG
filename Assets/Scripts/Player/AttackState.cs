using UnityEngine;

public class AttackState : IState<PlayerCtrl>
{
    Detectionarea[] _hitBox;
    public void Enter(PlayerCtrl player)
    {
        _hitBox = player.HitBox;
        DisableAll(); //진입시 다꺼두고

        player.Anima.applyRootMotion = true;
        player.Anima.SetTrigger("Attack");
        _hitBox[0].enabled = true;
    }

    public void Execute(PlayerCtrl player)
    {
        
    }

    public void Exit(PlayerCtrl player)
    {
        DisableAll();
        player.Anima.applyRootMotion = false;
        
    }

    void DisableAll()   //모든 히트박스 끄기
    {
        foreach (var hitBox in _hitBox)
            hitBox.enabled = false;
    }

}
