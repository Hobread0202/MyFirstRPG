using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour
{
    [SerializeField] Transform _target; //타겟

    BoxCollider _boxCol;   //탐지용
    CapsuleCollider _capCol; //판정용
    NavMeshAgent _nav; //길찾기
    Material _mat;  //색변환용

    //상태
    IState<EnemyCtrl> _currentState;
    EnemyMoveState _enemyMoveState;
    EnemyIdleState _enemyIdleState;

    //프로퍼티
    public EnemyMoveState EnemyMoveState => _enemyMoveState;
    public EnemyIdleState EnemyIdleState => _enemyIdleState;
    public NavMeshAgent Nav => _nav;
    public Transform Target => _target;

    private void Awake()
    {
        _boxCol = GetComponent<BoxCollider>();
        _capCol = GetComponent<CapsuleCollider>();
        _nav = GetComponent<NavMeshAgent>();
        _mat = GetComponent<Material>();

        _enemyMoveState = new EnemyMoveState();
        _enemyIdleState = new EnemyIdleState();

        _currentState = _enemyIdleState;
        _currentState.Enter(this);
    }

    private void Update()
    {
        _currentState.Execute(this);
    }


    public void ChangeState(IState<EnemyCtrl> newState)
    {
        if (_currentState == newState) return;

        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }
    
}
