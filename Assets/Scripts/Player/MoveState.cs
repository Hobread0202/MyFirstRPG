using UnityEngine;

public class MoveState : IState<PlayerCtrl>
{    
    float _currentSpeed; //현재속도 계산용 변수
    float _currentMoveY; //애니메이션 파라미터값
    public void Enter(PlayerCtrl player)
    {
        //진행중이던 파라미터값 받아오기
        _currentMoveY = player.Anima.GetFloat("MoveY");
    }

    public void Execute(PlayerCtrl player)
    {
        Vector2 input = player.MoveInput;
        float speed = player.PlayerStats.speed; //플레이어 기본 이속

        _currentMoveY = Mathf.Lerp(_currentMoveY, input.y, Time.deltaTime * 5f);        
        player.Anima.SetFloat("MoveY", _currentMoveY);

        //회전 로직
        if (input.x != 0)
        {
            player.transform.Rotate(Vector3.up * input.x * 150f * Time.deltaTime);
        }



        //뒤로가는중이면 속도 50%감소
        if (input.y < 0) { _currentSpeed = speed * 0.5f; }

        //아니면 그대로 진행
        else { _currentSpeed = speed; }

        //이동값 계산
        Vector3 moveDir = player.transform.forward * input.y * _currentSpeed;

        //중력값 덮어쓰기
        moveDir.y = player.Gravity;

        //적용
        player.ChaCtrl.Move(moveDir * Time.deltaTime);


    }

    public void Exit(PlayerCtrl player)
    {
    }
}
