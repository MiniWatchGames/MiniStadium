using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

// 추가되는 스킬은 여기에도 추가작성 필요
public enum ActionState {
    Idle,
    Attack,
    Hit,
    Dead,
    MovementSkills,
    WeaponSkills,
    None
}

public enum PostureState { 
    Idle,
    Crouch,
    None
}

public enum MovementState
{
    Idle,
    Walk,
    Jump,
    None
}

public enum StatType
{
    MaxHp,
    Defence,
    MoveSpeed,
    JumpPower
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IInputEvents , IStatObserver
{
    [SerializeField] private LayerMask groundLayer;
    
    private CharacterController _characterController;
    private CameraController _cameraController;
    private const float _gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
    
    
    // input값 저장, 전달 
    private Vector2 _currentMoveInput;
    public Vector2 CurrentMoveInput => _currentMoveInput;
    private float _lastInputTime;
    private float _inputBufferTime = 0.1f; // 100ms의 버퍼 타임




    // --------
    // 스탯 관련
    [Header("Stat")]
    [SerializeField] private float fixedFirstMaxHp;
    [SerializeField] private float fixedFirstDefence;
    [SerializeField] private float fixedFirstMoveSpeed;
    [SerializeField] private float fixedFirstJumpPower;

    private Stat baseMaxHp;
    private Stat baseDefence;
    private Stat baseMoveSpeed;
    private Stat baseJumpPower;

    public float BaseMaxHp => baseMaxHp.Value;
    public float BaseDefence => baseDefence.Value;
    public float BaseMoveSpeed => baseMoveSpeed.Value;
    public float BaseJumpPower => baseJumpPower.Value;


    private Dictionary<StatType,Stat> statDictionary;
    
    private ObservableFloat currentHp;
    public float CurrentHp
    {
        get => currentHp.Value;
        set
        {
            currentHp.Value = value;
            if (currentHp.Value <= 0)
            {
                Debug.Log("주금..");
            }
        }
    }

    // --------
    // 패시브 스킬관련
    [Header("Passive_Skill")]
    private PassiveFactory _passiveFactory;
    private List<IPassive> _passiveList;

    public PassiveFactory PassiveFactory => _passiveFactory;
    public List<IPassive> PassiveList => _passiveList;

    // --------
    // 상태 관련
    [Header("FSM")]
    private PlayerFSM<MovementState> _movementFsm;
    private PlayerFSM<PostureState> _postureFsm;
    private PlayerFSM<ActionState> _actionFsm;
    [SerializeField] private string defaultState;

    public PlayerFSM<MovementState> MovementFsm { get => _movementFsm; }
    public PlayerFSM<PostureState> PostureFsm { get => _postureFsm; }
    public PlayerFSM<ActionState> ActionFsm { get => _actionFsm; }

    // --------
    // 카메라 관련
    [Header("Camera")]
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    private float _yaw = 0f;
    private float _pitch = 0f;
    

    [Header("Weapon")] 
    private PlayerWeapon _playerWeapon;

    public PlayerWeapon PlayerWeapon { get => _playerWeapon; }

    [Header("Combat")]
    [SerializeField] private CombatManager combatManager;
    public CombatManager CombatManager { get => combatManager; }

    // --------
    // 애니메이션 관련 
    [SerializeField] private RuntimeAnimatorController swordController;
    [SerializeField] private RuntimeAnimatorController gunController;
    private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    public Animator Animator { get; private set; }
    public bool IsGrounded
    {
        get
        {
            return GetDistanceToGround() <= 0.03f;
        }
    }
    
    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _playerWeapon = GetComponent<PlayerWeapon>();
        
        _movementFsm = new PlayerFSM<MovementState>(StateType.Move, _playerWeapon, defaultState);
        _postureFsm = new PlayerFSM<PostureState>(StateType.Posture, _playerWeapon, defaultState);
        _actionFsm = new PlayerFSM<ActionState>(StateType.Action, _playerWeapon, defaultState);
    }

    private void Start()
    {
        // InputManager 구독 
        InputManager.instance.Register(this);

        Init();
      

        Debug.Log(baseMaxHp.Value);


        AddStatDecorate(StatType.MoveSpeed, 1f);

        AddStatDecorate(StatType.MaxHp, 3);

        CurrentHp -= 4;
        RemoveStatDecorate(StatType.MaxHp);
    }

    private void Update()
    {   
        _movementFsm.CurrentStateUpdate();
        _postureFsm.CurrentStateUpdate();
        _actionFsm.CurrentStateUpdate();
        DrawRay();
    }

    private void Init()
    {
        // 카메라 설정
        _cameraController = Camera.main.GetComponent<CameraController>();
        _cameraController.SetTarget(transform);
        _cameraController.SetSpineTarget(rotationTarget);
        _cameraController.IsIdle = IsIdle;

        //플레이어 스텟 설정
        currentHp = new ObservableFloat(fixedFirstMaxHp,"currentHp");
        statDictionary = new Dictionary<StatType, Stat>();
        baseMaxHp = new Stat(fixedFirstMaxHp,"baseMaxHp");
        statDictionary.Add(StatType.MaxHp, baseMaxHp);
        baseDefence = new Stat(fixedFirstDefence, "baseDefence");
        statDictionary.Add(StatType.Defence, baseDefence);
        baseMoveSpeed = new Stat(fixedFirstMoveSpeed, "baseMoveSpeed");
        statDictionary.Add(StatType.MoveSpeed, baseMoveSpeed);
        baseJumpPower = new Stat(fixedFirstJumpPower, "baseJumpPower");
        statDictionary.Add(StatType.JumpPower, baseJumpPower);
       
        
        // 구매내역에 따른 스텟 분배

        // 무기 설정 
        EquipWeapon(_playerWeapon);
        
        _movementFsm.Run(this);
        _postureFsm.Run(this);
        _actionFsm.Run(this);

        // 스텟 + currentHp 옵저버 등록 
        foreach (var stat in statDictionary)
        {
            stat.Value.AddObserver(this);
        }

        currentHp.AddObserver(this);


        //구매 목록에 따른 패시브 적용
        //Factory를 따로 두어 사용하는게 적절해 보이지만 일단 테스트를 위해 여기에 작성했음
        _passiveFactory = new PassiveFactory();
        _passiveList = new List<IPassive>();

        //임시, 구매내역이라 치고 작성
        List<string> passiveNames = new List<string>();
        passiveNames.Add("HpRegenerationPassive");
        passiveNames.Add("IncreasingRandomStatEvery20Seconds");
        _passiveFactory.CreatePassive(this,passiveNames);
        foreach (var passive in PassiveList)
        {
            passive.ApplyPassive(this);
        }
        //스킬 목록 적용
        //플레이어의 ActionFsm 내에 상태를 넣어야 함
        //AddSkillState에 넣을 스킬 목록을 집어넣으면 알아서 State가 생성됨
        //단 ActionState과 SkillFactory에 등록해두어야 추가 가능
        ActionFsm.AddSkillState(new List<string> { "MovementSkills" });
    }

    public void SetMovementState(string stateName)
    {
        _movementFsm.ChangeState(stateName, this);
    }
    public void SetPostureState(string stateName)
    {
        _postureFsm.ChangeState(stateName, this);
    }
    public void SetActionState(string stateName)
    {
        _actionFsm.ChangeState(stateName, this);
    }
    
    private void EquipWeapon(PlayerWeapon weapon)
    {
        // 무기 메쉬 교체 (구현 예정)
        
        // 애니메이터 교체 
        ApplyAnimatorController(weapon.WeaponType);
        // 무기별 전략 결정 
        combatManager.SetWeaponType(weapon.WeaponType);
    }

    private void OnAnimatorMove()
    {
        Vector3 movePosition;
        movePosition = Animator.deltaPosition;


        if (IsGrounded)
        {
            movePosition = Animator.deltaPosition;
        }
        else
        {
            movePosition = _characterController.velocity * Time.deltaTime;
        }
        
        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
        movePosition.y = _velocity.y * Time.deltaTime;
        _characterController.Move(movePosition);
    }
    
    #region Input_Events
    
    public void OnMove(Vector2 input)
    {
        if (IsGrounded)
        {
            // 이동 
            if (input != Vector2.zero)
            {
                _lastInputTime = Time.time;
                if (_movementFsm.CurrentState != MovementState.Walk)
                {
                    SetMovementState("Walk");
                }
                _currentMoveInput = input;
            }
            else
            {
                // 버퍼 시간 내에 입력이 없으면 Idle로 전환
                if (Time.time - _lastInputTime > _inputBufferTime)
                {
                    if (_movementFsm.CurrentState != MovementState.Idle)
                    {
                        SetMovementState("Idle");
                    }
                }
                // 버퍼 시간 내에는 이전 입력 유지
            }

        }
    }

    public void OnLook(Vector2 delta)
    {
        // 마우스 회전 
        // 마우스 감도 적용
        _yaw += delta.x * rotationSpeed;
        _pitch -= delta.y * rotationSpeed;
    
        // 수직 회전 각도 제한
        _pitch = Mathf.Clamp(_pitch, minAngle, maxAngle);
    
        // 카메라 Pitch 업데이트
        // 수직 회전의 일부를 상체에 적용 (전체 피치의 일정 비율)
        _cameraController.SetPitch(_pitch);
        _cameraController.SetYaw(_yaw);
    }

    public void OnJumpPressed()
    {
        // 점프 
        if (IsGrounded)
        {
            _velocity.y = Mathf.Sqrt(BaseJumpPower * -2f * _gravity);
            SetMovementState("Jump");
        }
    }

    public void OnFirePressed()
    {
        // 공격 (마우스 다운)
        combatManager.StartAttack();
        SetActionState("Attack");
    }

    public void OnFireReleased()
    {
        // 공격 (마우스 업)
        if (_actionFsm.CurrentState == ActionState.Attack)
        {
            combatManager.ProcessInput(false, false);
            SetActionState("Idle");
        }
    }

    public void OnCrouchPressed()
    {
        // 앉기 
        SetPostureState(_postureFsm.CurrentState == PostureState.Idle ? "Crouch" : "Idle");
    }
    
    #endregion
    
    // 바닥과 거리를 계산하는 함수
    public float GetDistanceToGround()
    {
        float maxDistance = 10f;
        if (Physics.Raycast(_characterController.transform.position, 
                Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }
    
    // 현재 무기 타입에 맞는 애니메이터 컨트롤러 적용
    public void ApplyAnimatorController(WeaponType weaponType)
    {
        if (Animator == null)
        {
            Debug.LogError("Player Animator not found!");
            return;
        }
        
        // 무기 타입에 따라 컨트롤러 선택
        RuntimeAnimatorController controllerToApply = null;
        
        switch (weaponType)
        {
            case WeaponType.Sword:
                controllerToApply = swordController;
                break;
            case WeaponType.Gun:
                controllerToApply = gunController;
                break;
        }
        
        // 컨트롤러 적용
        if (controllerToApply != null)
        {
            Animator.runtimeAnimatorController = controllerToApply;
            Debug.Log($"Applied {weaponType} animator controller");
        }
        else
        {
            Debug.LogWarning($"No animator controller assigned for weapon type: {weaponType}");
        }
    }

    #region 상태 체크 메소드 
    public bool IsIdle(out Action turningStep,float accumulatedYaw) {
        if (_movementFsm.CurrentState != MovementState.Idle)
        {

            turningStep = () =>
            {
                Animator.SetLayerWeight(1, 0f);
                Animator.SetBool("IsRightTurn", false);
                Animator.SetBool("IsLeftTurn", false);
            };
            return true;
        }
        else
        {

            if (accumulatedYaw > 90f)
            {
                turningStep = () =>
                {
                    Animator.SetLayerWeight(1, 0.35f);
                    Animator.SetBool("IsRightTurn", true);
                };
            }
            else if (accumulatedYaw < -30f)
            {
                turningStep = () =>
                {
                    Animator.SetLayerWeight(1, 0.35f);
                    Animator.SetBool("IsLeftTurn", true);
                };
            }
            else
            {

                turningStep = () =>
                {
                    Animator.SetLayerWeight(1, 0.35f);
                    Animator.SetBool("IsRightTurn", false);
                    Animator.SetBool("IsLeftTurn", false);
                };
            }
            return false;
        }
    }
    public bool IsWalking()
    {
        if (_movementFsm.CurrentState == MovementState.Walk)
        {

            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion


    #region 스텟 관련 메소드

    /// <summary>
    /// 지정 스탯에 데코레이터 메소드 추가
    /// </summary>
    /// <param name="stat">증가시킬 변수</param>
    /// <param name="additionalValue">증가시킬 양</param>
    public void AddStatDecorate(StatType stat, float additionalValue)
    {
        if (statDictionary.TryGetValue(stat, out var target))
        {
            target.AddDecorate(((value) => value + additionalValue));
        }
        if(stat == StatType.MaxHp)
        {
            if (CurrentHp >= baseMaxHp.Value)
            {
                CurrentHp = baseMaxHp.Value;
            }
            Debug.Log($"버프 적용 현재 체력{CurrentHp}");
        }
    }
    /// <summary>
    /// 지정 인덱스의 가장 마지막에 추가된 데코레이트 삭제
    /// </summary>
    /// <param name="stat">삭제시킬 변수</param>
    public void RemoveStatDecorate(StatType stat)
    {
        if (statDictionary.TryGetValue(stat, out var target))
        {
            target.RemoveModifiers();
        }
        if (stat == StatType.MaxHp)
        {
            if (CurrentHp >= baseMaxHp.Value)
            {
                CurrentHp = baseMaxHp.Value;
            }
            Debug.Log($"버프 적용 현재 체력{CurrentHp}");
        }
    }
    /// <summary>
    /// 지정 인덱스의 모든 데코레이트 삭제
    /// </summary>
    /// <param name="stat">삭제할 func의 인덱스</param>
    public void RemoveStatAllDecorate(StatType stat)
    {
        if (statDictionary.TryGetValue(stat, out var target))
        {
            target.RemoveAllModifiers();
        }
        if (stat == StatType.MaxHp)
        {
            if (CurrentHp >= baseMaxHp.Value)
            {
                CurrentHp = baseMaxHp.Value;
            }
            Debug.Log($"버프 적용 현재 체력{CurrentHp}");
        }
    }

    public void ChangeMovementSpeed(float value) {
        this.Animator.SetFloat(MoveSpeedHash, value);
    }


    //구매 내역 에 따른 스탯 분배 메소드 필요
    //구매 내역에 따른 Passive 생성, 스킬 생성도 필요
    // 너무 커질 것 같으니 PurchaseManager에서 관리하는 것도 좋을듯

    //정비소 측에서 구매내역을 넘겨받는 메소드 구현 필요

    #endregion


    // 디버깅 용 Ray
    private void DrawRay()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * 5f, Color.red);
    }

    #region 옵저버
    public void WhenStatChanged((float,string) data)
    {
      Debug.Log($"{data.Item2}가 {data.Item1}로 변경되었습니다.");
        switch (data.Item2) {
            case "baseMoveSpeed":
                ChangeMovementSpeed(data.Item1);
                break;
        }
    }
    #endregion
}
