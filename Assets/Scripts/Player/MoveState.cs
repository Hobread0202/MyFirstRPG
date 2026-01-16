using UnityEngine;

public class MoveState : IState<PlayerCtrl>
{
    public void Enter(PlayerCtrl player)
    {
        
    }

    public void Execute(PlayerCtrl player)
    {
        Vector2 input = player.MoveInput;
        float speed = player.PlayerStats.Speed; //플레이어 기본 이속
        float currentSpeed; //현재속도 계산용 변수


        player.Anima.SetFloat("MoveY", input.y);

        //회전 로직
        if (input.x != 0)
        {
            player.transform.Rotate(Vector3.up * input.x * 150f * Time.deltaTime);
        }



        //뒤로가는중이면 속도 50%감소
        if (input.y < 0) { currentSpeed = speed * 0.5f; }

        //아니면 그대로 진행
        else { currentSpeed = speed; }

        //이동값 계산
        Vector3 moveDir = player.transform.forward * input.y * currentSpeed;

        //중력값 덮어쓰기
        moveDir.y = player.Gravity;

        //적용
        player.ChaCtrl.Move(moveDir * Time.deltaTime);


    }

    public void Exit(PlayerCtrl player)
    {
        //상태 나갈때 초기화
        player.Anima.SetFloat("MoveY", 0);
    }
}
