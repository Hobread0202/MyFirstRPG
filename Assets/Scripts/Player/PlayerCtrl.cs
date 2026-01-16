using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] PlayerStats _playerStats;
    Animator _anima;
    CharacterController _chaCtrl;
    Vector2 _moveInput;

    public float Gravity;   //중력
    bool _isCombo = false;



    //상태
    IState<PlayerCtrl> _currentState;
    MoveState _moveState;
    IdleState _idleState;
    AttackState _attackState;

    
    


    //프로퍼티
    public Vector2 MoveInput => _moveInput;
    public CharacterController ChaCtrl => _chaCtrl;
    public PlayerStats PlayerStats => _playerStats;
    public Animator Anima => _anima;
    public MoveState MoveState => _moveState;
    public IdleState IdleState => _idleState;
    public AttackState AttackState => _attackState;

    

    private void Awake()
    {
        _anima = GetComponent<Animator>();
        _chaCtrl = GetComponent<CharacterController>();

        _moveState = new MoveState();
        _idleState = new IdleState();
        _attackState = new AttackState();
        _currentState = _idleState;
        _currentState.Enter(this);


    }

    private void Update()
    {
        _currentState.Execute(this);

        if (_chaCtrl.isGrounded && Gravity < 0)
            Gravity = -1f;
        else
            Gravity += Physics.gravity.y * Time.deltaTime;

        if (_currentState != _moveState)
        {
            _chaCtrl.Move(new Vector3(0, Gravity, 0) * Time.deltaTime);
        }



        ////바닥 체크
        //if (player.ChaCtrl.isGrounded == false)
        ////공중이면 y축 중력가하기
        //{ player.Gravity += Physics.gravity.y * Time.deltaTime; }
    }


    public void ChangeState(IState<PlayerCtrl> newState)
    {
        if (_currentState == newState) return;

        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }

    

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();

        //공격중에 상태전환 막기
        if (_currentState == _attackState) return;

        if (ctx.performed)
            ChangeState(_moveState);

        else
        {
            _anima.SetFloat("MoveY", 0f);
            ChangeState(_idleState);
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (_currentState != _attackState)
            {
                ChangeState(_attackState);
            }
        }
    }

    public void OnAttackFinished()  // 공격이 끝났을 때만 호출되는 함수
    {
        if (_moveInput.sqrMagnitude > 0)
            ChangeState(_moveState);
        else
            ChangeState(_idleState);
    }

}
