using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour, IDamageable
{    
    int _currentHp;
    [SerializeField] MonsterData _enemyStats;
    [SerializeField] float _angularSpeed = 200f;    //회전속도
    [SerializeField] Transform _target; //타겟
    [SerializeField] Transform _spawnArea; //돌아갈 스폰지점
    [SerializeField] Detectionarea _detectionArea;  //탐지할구역

    Animator _anima;
    CapsuleCollider _capCol; //판정용
    NavMeshAgent _nav; //길찾기
    Material _mat;  //색변환용

    //상태
    IState<EnemyCtrl> _currentState;
    EnemyMoveState _enemyMoveState;
    EnemyIdleState _enemyIdleState;
    EnemyDeadState _enemyDeadState;

    //프로퍼티
    public EnemyMoveState EnemyMoveState => _enemyMoveState;
    public EnemyIdleState EnemyIdleState => _enemyIdleState;
    public EnemyDeadState EnemyDeadState => _enemyDeadState;
    public NavMeshAgent Nav => _nav;
    public Transform Target => _target;
    public Transform SpawnArea => _spawnArea;
    public Animator Anima => _anima;

    public TeamEnum TeamType => TeamEnum.Enemy;

    private void Awake()
    {

        //컴포넌트 가져오기
        _anima = GetComponent<Animator>();
        _capCol = GetComponent<CapsuleCollider>();
        _nav = GetComponent<NavMeshAgent>();
        _mat = GetComponent<Material>();

        //상태생성
        _enemyMoveState = new EnemyMoveState();
        _enemyIdleState = new EnemyIdleState();
        _enemyDeadState = new EnemyDeadState();

        //상태초기화
        _currentState = _enemyIdleState;
        _currentState.Enter(this);

        //이벤트 구독
        _detectionArea.OnTargetEnter += FollowPlayer;
        _detectionArea.OnTargetExit += RetrunArea;
    }
    private void Start()
    {
        _currentHp = _enemyStats.maxHp;
        _nav.speed = _enemyStats.speed;
        _nav.updateRotation = true; // 이동 방향으로 자동 회전
        _nav.angularSpeed = _angularSpeed; // 회전 속도
    }

    private void Update()
    {
        _currentState.Execute(this);
    }

    private void OnDestroy()
    {
        _detectionArea.OnTargetEnter -= FollowPlayer;
        _detectionArea.OnTargetExit -= RetrunArea;
    }

    public void ChangeState(IState<EnemyCtrl> newState)
    {
        if (_currentState == newState) return;

        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }

    void FollowPlayer(Collider player)
    {
        ChangeState(_enemyMoveState);
    }

    void RetrunArea(Collider player)
    {
        ChangeState(_enemyIdleState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerCtrl>(out PlayerCtrl player))
            {
                player.TakeDamage(_enemyStats.Damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        Debug.Log($"더지체력 {_currentHp}");

        if (_currentHp <= 0)
        {
            ChangeState(_enemyDeadState);
        }
    }
}
