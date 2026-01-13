using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] PlayerStats _playerStats;

    //상태
    IState<PlayerCtrl> _currentState;
    MoveState _moveState;
    IdleState _idleState;

    //카메라
    Transform _camPos;
    float _camSensitivity = 2f;
    float yaw;
    float pitch;

    Animator _anima;
    Vector2 _moveInput;


    //프로퍼티
    public Animator Anima => _anima;

    private void Awake()
    {
        _anima = GetComponent<Animator>();

        _moveState = new MoveState();
        _idleState = new IdleState();
        _currentState = _idleState;
        _currentState.Enter(this);


    }

    private void Update()
    {
        _currentState.Execute(this);
    }
    void ChangeState(IState<PlayerCtrl> newState)
    {
        if (_currentState == newState) return;
        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }



    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
}
