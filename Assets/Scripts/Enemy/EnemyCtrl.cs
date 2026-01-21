using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour, IDamageable
{    
    int _currentHp; //현제체력 체크용
    [SerializeField] MonsterData _enemyStats;
    [SerializeField] float _angularSpeed = 200f;    //회전속도
    [SerializeField] Detectionarea _detectionArea;  //탐지할구역

    EnemyType enemyType;    //몬스터타입
    EnemyPoolManager _enemyPool;    //오브젝트풀

    Animator _anima;
    CapsuleCollider _capCol; //판정용
    NavMeshAgent _nav; //길찾기
    Material _mat;  //색변환용
    Transform _target;  //추적할타겟
    Transform _spawnArea;   //돌아갈스폰구역



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
    public Animator Anima => _anima;
    public Transform Target => _target;
    public Transform SpawnArea => _spawnArea;

    public TeamEnum TeamType => TeamEnum.Enemy;

    private void Awake()
    {

        //컴포넌트 가져오기
        _anima = GetComponent<Animator>();
        _capCol = GetComponent<CapsuleCollider>();
        _nav = GetComponent<NavMeshAgent>();
        //_mat = GetComponent<Renderer>().material;

        //상태생성
        _enemyMoveState = new EnemyMoveState();
        _enemyIdleState = new EnemyIdleState();
        _enemyDeadState = new EnemyDeadState();

        //상태초기화
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
    public void SetTarget(Transform target)
    {
        _target = target;
    }
    public void SetSpawnArea(Transform spawnArea)
    {
        _spawnArea = spawnArea;
    }

    public void SetPoolManager(EnemyPoolManager manager)
    {
        //매니저설정
        _enemyPool = manager;
    }

    public void SetEnemyType(EnemyType type)
    {
        //타입설정
        enemyType = type;
    }

    public void OnSpawnFromPool() //스폰될때 설정
    {
        

        _currentHp = _enemyStats.maxHp;
        _nav.speed = _enemyStats.speed;
        _nav.updateRotation = true; //이동 방향으로 자동 회전
        _nav.angularSpeed = _angularSpeed; //회전 속도

        //이벤트 구독
        _detectionArea.OnTargetEnter += FollowPlayer;
        _detectionArea.OnTargetExit += RetrunArea;
    }
        
    public void OnReturnToPool() //풀로 반환될 때
    {
        _nav.ResetPath();   //기존 경로 제거
        _nav.velocity = Vector3.zero; //속도 0으로 초기화
        _nav.enabled = false;  //에이전트 비활성화
        _detectionArea.OnTargetEnter -= FollowPlayer;
        _detectionArea.OnTargetExit -= RetrunArea;
    }
}
