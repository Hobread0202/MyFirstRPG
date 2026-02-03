using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] PlayerData _playerStats;
    [SerializeField] Detectionarea[] _hitBox;
    Animator _anima;
    CharacterController _chaCtrl;
    Vector2 _moveInput;

    public float Gravity;   //중력
    int _currentDamage;
    int _currentHp;
    int _currentExp;
    int _currentLevel;
    bool _isDead;

    //bool _isCombo = false;



    //상태
    IState<PlayerCtrl> _currentState;
    MoveState _moveState;
    IdleState _idleState;
    AttackState _attackState;


    public event Action<float, float> OnHpChanged;
    public event Action OnDead;
    public event Action<int> OnLevelChanged;
    public event Action<float, float> OnExpChanged;


    //프로퍼티
    public Transform PlayerTransform => transform;
    public Vector2 MoveInput => _moveInput;
    public CharacterController ChaCtrl => _chaCtrl;
    public PlayerData PlayerStats => _playerStats;
    public Animator Anima => _anima;
    public MoveState MoveState => _moveState;
    public IdleState IdleState => _idleState;
    public AttackState AttackState => _attackState;
    public Detectionarea[] HitBox => _hitBox;

    public bool IsDead => _isDead;
    public int CurrentLevel => _currentLevel;
    public int CurrentExp => _currentExp;



    private void Awake()
    {
        _anima = GetComponent<Animator>();
        _chaCtrl = GetComponent<CharacterController>();

        _moveState = new MoveState();
        _idleState = new IdleState();
        _attackState = new AttackState();
        
    }

    private void Start()
    {
        //데이터 불러오기
        SaveData data = FirebaseDBMgr.Instance.LoadedData;

        //값셋팅
        _currentDamage = PlayerStats.Damage;

        //세이브데이터
        _currentHp = data.currentHp;
        _currentExp = data.currentExp;
        _currentLevel = data.currentLevel;

        OnHpChanged?.Invoke(_currentHp, _playerStats.maxHp);
        OnExpChanged?.Invoke(_currentExp, _playerStats.maxExp);
        OnLevelChanged?.Invoke(_currentLevel);

        Debug.Log($"시작할 때 체력: {_currentHp}");
        Debug.Log($"시작할 때 경치: {_currentExp}");

        _chaCtrl.enabled = false;
        transform.position = data.lastPos;
        _chaCtrl.enabled = true;
    }

    private void Update()
    {
        if (_isDead) return;

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
    private void OnEnable()
    {
        _currentState = _idleState;
        _currentState.Enter(this);

        //히트박스 배열이벤트 등록
        foreach (var hitBox in _hitBox)
        {
            hitBox.OnTargetEnter += HitDamage;
        }

    }

    private void OnDisable()
    {

        foreach (var hitBox in _hitBox)
        {
            hitBox.OnTargetEnter -= HitDamage;
        }

    }


    public void ChangeState(IState<PlayerCtrl> newState)
    {
        if (_currentState == newState) return;

        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHp -= damage;
        Debug.Log(_currentHp);

        OnHpChanged?.Invoke(_currentHp, _playerStats.maxHp);

        if (_currentHp <= 0)
        {
            _isDead = true;
            _anima.SetTrigger("Die");
            OnDead?.Invoke();
        }
    }

    public void ExpUp()
    {
        _currentExp += 10;

        //UI에게 알림 보내기
        OnExpChanged?.Invoke(_currentExp, _playerStats.maxExp);

        if (_currentExp >= _playerStats.maxExp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        _playerStats.maxExp += 100;
        _currentExp = 0;
        _currentLevel++;

        //UI에게 알림 보내기
        OnLevelChanged?.Invoke(_currentLevel);
        OnExpChanged?.Invoke(_currentExp, _playerStats.maxExp);
    }
    public void OnAttackFinished()  // 공격이 끝났을 때만 호출되는 함수
    {
        if (_moveInput.sqrMagnitude > 0)
            ChangeState(_moveState);
        else
            ChangeState(_idleState);
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
            ChangeState(_idleState);
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //커서켜져있으면 공격 불가
            if (Cursor.visible) return;

            if (_currentState != _attackState)
            {
                ChangeState(_attackState);
            }
        }
    }

    void HitDamage(Collider enemy)
    {
        //체크후 가져오기
        if (enemy.TryGetComponent<EnemyCtrl>(out EnemyCtrl enemyCtrl))
        {
            //반환된 불값 저장
            bool kill = enemyCtrl.TakeDamage(_currentDamage, gameObject.transform.position);

            //죽였으면 실행
            if (kill)
            {
                ExpUp();
            }
        }
    }

    //현재 데이터 반환함수
    public SaveData GetCurrentSaveData()
    {
        SaveData data = new SaveData();
        data.lastSceneName = SceneManager.GetActiveScene().name;
        data.lastX = transform.position.x;
        data.lastY = transform.position.y;
        data.lastZ = transform.position.z;
        data.currentHp = _currentHp;
        data.currentExp = _currentExp;
        data.currentLevel = _currentLevel;

        return data;
    }

    public void ResetPlayer(int hp)
    {
        _currentHp = hp;
        _isDead = false;
        _anima.Rebind();
        _chaCtrl.enabled = true;
        OnHpChanged?.Invoke(_currentHp, _playerStats.maxHp);
    }
}
