using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour
{    
    int _currentHp; //현제체력 체크용
    bool _isDead;
    [SerializeField] MonsterData _enemyStats;
    [SerializeField] float _angularSpeed = 200f;    //회전속도
    [SerializeField] Detectionarea _detectionArea;  //탐지할구역
    [SerializeField] EnemyHpUI _hpUI;

    EnemyType enemyType;    //몬스터타입
    EnemyPoolManager _enemyPool;    //오브젝트풀

    Animator _anima;
    Collider[] _col; //판정용
    NavMeshAgent _nav; //길찾기
    Material _mat;  //색변환용
    Transform _target;  //추적할타겟
    Transform _spawnArea;   //돌아갈스폰구역
    Coroutine _isKnockback; //넉백 체크용 변수

    public event Action OnDead;

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
    public MonsterData EnemyData => _enemyStats;

    public TeamEnum TeamType => TeamEnum.Enemy;

    private void Awake()
    {

        //컴포넌트 가져오기
        _anima = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _col = GetComponentsInChildren<Collider>();

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
        _target = player.transform;
        ChangeState(_enemyMoveState);
        _hpUI.Show(player.transform);
    }

    void RetrunArea(Collider player)
    {
        _target = null;
        ChangeState(_enemyIdleState);
        _hpUI.Hide();
    }

    public bool TakeDamage(int damage, Vector3 hitPos)
    {
        //죽은상태면 리턴
        if (_isDead) return false;

        _currentHp -= damage;
        //ui갱신
        _hpUI.TakeDamage(_currentHp, _enemyStats.maxHp);

        if (_currentHp <= 0)
        {
            OnDead?.Invoke();
            //참으로 바꾸고
            _isDead = true;
            //트리거 리셋시키고
            _anima.ResetTrigger("Hit");
            //죽은상태전환
            ChangeState(_enemyDeadState);

            //죽었으면 참반환
            return true;
        }

        //아니면 피격모션
        _anima.SetTrigger("Hit");

        StartKnockback(hitPos);
        Debug.Log($"더지체력 {_currentHp}");

        return false;


    }

    //넉백중인지 체크후 넘기는 함수
    void StartKnockback(Vector3 hitPoint)
    {
        if (_isKnockback != null)
            StopCoroutine(_isKnockback);

        _isKnockback = StartCoroutine(KnockbackCoroutine(hitPoint));
    }


    IEnumerator KnockbackCoroutine(Vector3 hitPoint)
    {
        // 충돌나니깐 끄기
        _nav.enabled = false;

        Vector3 dir = (transform.position - hitPoint).normalized;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + dir * 1.5f;

        float time = 0f;

        while (time < 0.15f)
        {
            time += Time.deltaTime;
            float t = time / 0.15f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;

        //네비메쉬 다시 켜기
        _nav.enabled = true;
    }


    //피격모션이벤트용함수
    public void OnHitFinished()
    {
        if (_isDead) return;

        ChangeState(_enemyIdleState);
        Debug.Log("상태전환");
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
        _isDead = false;
        _target = null;
        ChangeState(_enemyIdleState);

        _currentHp = _enemyStats.maxHp;
        _nav.speed = _enemyStats.speed;
        _nav.updateRotation = true; //이동 방향으로 자동 회전
        _nav.angularSpeed = _angularSpeed; //회전 속도
        _hpUI.SetHp();
        

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

    public void OnDeadFinished()    //죽음모션끝나면 호출
    {
        _enemyPool.ReturnEnemy(enemyType, this);
    }


    //모든 콜라이더 키고 끄기
    public void DisableAllColliders()
    {
        foreach (var col in _col)
        {
            col.enabled = false;
        }
    }

    
    public void EnableAllColliders()
    {
        foreach (var col in _col)
        {
            col.enabled = true;
        }
    }


    //감지범위체크용 함수
    public bool TargetInDetectionArea()
    {
        //탐지범위, 타겟체크하고 타겟 콜라이더가 범위안에 있는지 체크
        return _detectionArea != null && _target != null && _detectionArea.IsColliderInside(_target.GetComponent<Collider>());
    }
}
