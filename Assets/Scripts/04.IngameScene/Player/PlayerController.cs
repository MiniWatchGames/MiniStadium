using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

// 추가되는 스킬은 여기에도 추가작성 필요
public enum ActionState
{
    Idle,
    Attack,
    Hit,
    Dead,
    MovementSkills,
    WeaponSkills,
    RunSkill,
    DoubleJumpSkill,
    TeleportSkill,
    None
}

public enum PostureState
{
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
public class PlayerController : NetworkBehaviour, IInputEvents, IDamageable, IStatObserver
{
    [SerializeField] private Transform head;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask groundMask;
    private CharacterController _characterController;
    private const float Gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
    private bool _isDead = false;
    private PlayerItems _playerItems;
    private PurchaseManager _purchaseManager;
    public PurchaseManager PurchaseManager { get => _purchaseManager; set => _purchaseManager = value; }

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

    // 스탯 증가량
    [SerializeField] private float maxHpIncreaseAmount;
    [SerializeField] private float defenceIncreaseAmount;
    [SerializeField] private float moveSpeedIncreaseAmount;
    [SerializeField] private float jumpPowerIncreaseAmount;

    private Stat baseMaxHp;
    private Stat baseDefence;
    private Stat baseMoveSpeed;
    private Stat baseJumpPower;

    public Stat BaseMaxHp => baseMaxHp;
    public Stat BaseDefence => baseDefence;
    public Stat BaseMoveSpeed => baseMoveSpeed;
    public Stat BaseJumpPower => baseJumpPower;

    private Dictionary<StatType, Stat> statDictionary;

    //캐릭터가 죽거나 적을 죽였을때 이 이벤트가 발생한다
    public Action<GameObject> OnPlayerDie;
    public Action<GameObject> OnEnemyKilled;
    
    private ObservableFloat currentHp;

    public ObservableFloat CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            if (currentHp.Value <= 0 && !_isDead)
            {
                Debug.Log("주금..");
                _isDead = true;
                OnPlayerDie.Invoke(gameObject);
                SetMovementStateServer("Idle", this);
                SetPostureStateServer("Idle", this);
                SetActionStateServer("Dead", this);
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

    private List<(ActionState, IPlayerState)> _weaponSkills;
    private List<(ActionState, IPlayerState)> _movementSkills;

    private string _firstWeaponSkill;
    private string _secondWeaponSkill;
    private string _firstMoveSkill;
    private string _secondMoveSkill;

    private float _firstWeaponSkillCoolTime;
    private float _secondWeaponSkillCoolTime;
    private float _firstMovementSkillCoolTime;
    private float _secondMovementSkillCoolTime;

    private ObservableFloat _currentFirstWeaponSkillCoolTime;
    private ObservableFloat _currentSecondWeaponSkillCoolTime;
    private ObservableFloat _currentFirstMovementSkillCoolTime;
    private ObservableFloat _currentSecondMovementSkillCoolTime;

    public ObservableFloat CurrentFirstWeaponSkillCoolTime { get; }
    public ObservableFloat CurrentSecondWeaponSkillCoolTime { get; }
    public ObservableFloat CurrentFirstMovementSkillCoolTime { get; }
    public ObservableFloat CurrentSecondMovementSkillCoolTime { get; }

    private Coroutine _firstWeaponSkillCoolTimeCoroutine;
    private Coroutine _secondWeaponSkillCoolTimeCoroutine;
    private Coroutine _firstMovementSkillCoolTimeCoroutine;
    private Coroutine _secondMovementSkillCoolTimeCoroutine;

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
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float rotationSmoothSpeed = 30f; // 회전 부드러움 정도
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    private float _yaw = 0f;
    private float _pitch = 0f;
    public CameraController CameraController { get => cameraController; }

    [Header("Weapon")]
    private PlayerWeapon _playerWeapon;
    public PlayerWeapon PlayerWeapon { get => _playerWeapon; }

    [Header("Combat")]
    private CombatManager _combatManager;
    public CombatManager CombatManager { get => _combatManager; }

    // --------
    // 애니메이션 관련 
    [Header("Animation")]
    [SerializeField] private RuntimeAnimatorController swordAnimatorController;
    [SerializeField] private RuntimeAnimatorController gunAnimatorController;
    [SerializeField] private float aimWeight = 1f; // IK 가중치 (0-1)
    private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    public Animator Animator { get; private set; }

    public bool IsGrounded
    {
        get {return GetDistanceToGround() <= 0.03f; }
    }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _combatManager = GetComponent<CombatManager>();
        _playerWeapon = GetComponent<PlayerWeapon>();

        _purchaseManager = new PurchaseManager();

        _movementFsm = new PlayerFSM<MovementState>(StateType.Move, _playerWeapon, defaultState);
        _postureFsm = new PlayerFSM<PostureState>(StateType.Posture, _playerWeapon, defaultState);
        _actionFsm = new PlayerFSM<ActionState>(StateType.Action, _playerWeapon, defaultState);

        //플레이어 스텟 설정
        currentHp = new ObservableFloat(fixedFirstMaxHp, "currentHp");
        statDictionary = new Dictionary<StatType, Stat>();
        baseMaxHp = new Stat(fixedFirstMaxHp, "baseMaxHp");
        statDictionary.Add(StatType.MaxHp, baseMaxHp);
        baseDefence = new Stat(fixedFirstDefence, "baseDefence");
        statDictionary.Add(StatType.Defence, baseDefence);
        baseMoveSpeed = new Stat(fixedFirstMoveSpeed, "baseMoveSpeed");
        statDictionary.Add(StatType.MoveSpeed, baseMoveSpeed);
        baseJumpPower = new Stat(fixedFirstJumpPower, "baseJumpPower");
        statDictionary.Add(StatType.JumpPower, baseJumpPower);
        //Factory를 따로 두어 사용하는게 적절해 보이지만 일단 테스트를 위해 여기에 작성했음
        _passiveFactory = new PassiveFactory();
        _passiveList = new List<IPassive>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            cameraController.GameObject().SetActive(false);
            gameObject.GetComponent<PlayerController>().enabled = false;
            
        }
        ReInit();
    }

    private void Start()
    {
        //Init();      
        _movementFsm.Run(this);
        _postureFsm.Run(this);
        _actionFsm.Run(this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    private void Update()
    {
        _movementFsm?.CurrentStateUpdate();
        _postureFsm?.CurrentStateUpdate();
        _actionFsm?.CurrentStateUpdate();
        DrawRay();
    }
    [ServerRpc]
    public void SetMovementStateServer(string stateName, PlayerController player)
    {
        SetMovementState(stateName, player);
    }
    [ObserversRpc]
    public void SetMovementState(string stateName, PlayerController player)
    {
        _movementFsm.ChangeState(stateName, player);
    }
    
    [ServerRpc]
    public void SetPostureStateServer(string stateName, PlayerController player)
    {
        SetPostureState(stateName, player);
    }
    [ObserversRpc]
    public void SetPostureState(string stateName, PlayerController player)
    {
        _postureFsm.ChangeState(stateName, player);
    }
    
    [ServerRpc]
    public void SetActionStateServer(string stateName, PlayerController player)
    {
        SetActionState(stateName, player);
    }
    [ObserversRpc]
    public void SetActionState(string stateName, PlayerController player)
    {
        _actionFsm.ChangeState(stateName, player);
    }

    private void EquipWeapon(PlayerWeapon weapon)
    {
        // 무기 생성 
        _playerWeapon.CreateWeapon(weapon.WeaponType, _playerWeapon);
        // 무기에 맞는 애니메이터로 교체 
        ApplyAnimatorController(weapon.WeaponType);
        // 무기별 전략 결정 
        _combatManager.SetWeaponType(weapon.WeaponType);
    }

    // 데이지 계산
    public void TakeDamage(DamageInfo damageInfo)
    {
        // todo : 데미지 적용 식 정해야 함

        // 자기 자신이면 데미지 입지 않음
        var rootTransform = damageInfo.attacker.transform.root;
        var rootObject = rootTransform.gameObject;
        if (rootObject == gameObject) return;

        var damage = damageInfo.damage;
        currentHp.Value -= damage;
        Debug.Log($"current Hp = {currentHp.Value}");
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
        _velocity.y += Gravity * Time.deltaTime;
        movePosition.y = _velocity.y * Time.deltaTime;
        _characterController.Move(movePosition);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (Animator == null) return;


        // 현재 Spine 본의 애니메이션 회전값을 가져오기
        Transform spineBone = Animator.GetBoneTransform(HumanBodyBones.Spine);
        if (spineBone == null) return;

        // 애니메이션의 현재 회전
        Quaternion animatedRotation = spineBone.localRotation;


        // 애니메이션의 Y 및 Z 회전만 추출
        Vector3 animEuler = animatedRotation.eulerAngles;

        // 현재 애니메이션의 X 회전 (정규화)
        float animX = animEuler.x;
        if (animX > 180f) animX -= 360f;

        // 최종 X 회전
        float combinedX = animX + _pitch;

        // X 회전 클램프 적용
        float clampedX = Mathf.Clamp(combinedX, minAngle, maxAngle);

        // 최종 회전 생성
        Quaternion finalRotation = Quaternion.Euler(clampedX, 0, 0);

        // 최종 회전 적용
        Animator.SetBoneLocalRotation(HumanBodyBones.Spine, finalRotation);
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
                    SetMovementStateServer("Walk", this);
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
                        SetMovementStateServer("Idle", this);
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
        // 플레이어 몸통 전체를 yaw 방향으로 회전
        Quaternion targetBodyRotation = Quaternion.Euler(0, _yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetBodyRotation, Time.deltaTime * rotationSmoothSpeed);

        // 카메라 컨트롤러에 값 전달
        cameraController.UpdateCamera(_pitch, _yaw);
    }

    public void OnJumpPressed()
    {
        // 점프 
        if (IsGrounded)
        {
            Jump(); 
            SetMovementStateServer("Jump", this);
        }
    }
    public void Jump() {
        _velocity.y = Mathf.Sqrt(BaseJumpPower.Value * -2f * Gravity);
    }
    public void OnFirePressed()
    {
        // 공격 (마우스 다운)
        if (_actionFsm.CurrentState != ActionState.Attack)
        {
            SetActionStateServer("Attack", this);
        }
        else
        {
            _combatManager.ProcessInput(true, false);
        }
    }

    public void OnFireReleased()
    {
        // 공격 (마우스 업)
        if (_actionFsm.CurrentState == ActionState.Attack)
        {
            _combatManager.ProcessInput(false, false);
        }
    }

    public void OnCrouchPressed()
    {
        SetPostureStateServer(_postureFsm.CurrentState == PostureState.Idle ? "Crouch" : "Idle", this);
    }


    public void OnFirstWeaponSkillPressed()
    {
        if (_weaponSkills?.Count >= 1)
        {
            var weaponSkill = _weaponSkills[0];
            ISkillData weaponSkillData = weaponSkill.Item2 as ISkillData;

            if (_currentFirstWeaponSkillCoolTime?.Value > 0)
            {
                return;
            }
            else
            {
                if (weaponSkillData.CoolTime != null)
                {
                    _currentFirstWeaponSkillCoolTime.Value = _firstWeaponSkillCoolTime;
                    if (_firstWeaponSkillCoolTimeCoroutine == null)
                    {
                        _firstWeaponSkillCoolTimeCoroutine =
                        StartCoroutine(
                            SkillCoolDown(
                                _currentFirstWeaponSkillCoolTime, () => { _firstWeaponSkillCoolTimeCoroutine = null; }
                            )
                        );
                    }
                }
                _firstWeaponSkill = weaponSkill.Item1.ToString();
                if (_actionFsm.CurrentState != weaponSkill.Item1)
                    SetActionStateServer(_firstWeaponSkill, this);

                return;
            }
        }
    }

    public void OnFirstWeaponSkillReleased()
    {

        if (_weaponSkills?.Count >= 1)
        {
            if (_actionFsm.CurrentState == _weaponSkills[0].Item1)
            {
                SetActionStateServer("Idle", this);
            }
        }
    }

    public void OnSecondWeaponSkillPressed()
    {
        if (_weaponSkills?.Count >= 2)
        {
            if (_weaponSkills?.Count >= 1)
            {
                var weaponSkill = _weaponSkills[0];
                ISkillData weaponSkillData = weaponSkill.Item2 as ISkillData;

                if (_currentSecondWeaponSkillCoolTime?.Value > 0)
                {
                    return;
                }
                else
                {
                    if (weaponSkillData.CoolTime != null)
                    {
                        _currentSecondWeaponSkillCoolTime.Value = _secondWeaponSkillCoolTime;
                        if (_secondWeaponSkillCoolTimeCoroutine == null)
                        {
                            _secondWeaponSkillCoolTimeCoroutine =
                            StartCoroutine(
                                SkillCoolDown(
                                    _currentSecondWeaponSkillCoolTime, () => { _secondWeaponSkillCoolTimeCoroutine = null; }
                                )
                            );
                        }
                    }
                    _secondWeaponSkill = weaponSkill.Item1.ToString();
                    if (_actionFsm.CurrentState != weaponSkill.Item1)
                        SetActionStateServer(_secondWeaponSkill, this);

                    return;
                }
            }
        }
    }

    public void OnSecondWeaponSkillReleased()
    {
        if (_weaponSkills?.Count >= 2)
        {
            if (_actionFsm.CurrentState == _weaponSkills[1].Item1)
            {
                SetActionStateServer("Idle", this);
            }
        }
    }

    public void OnFirstMoveSkillPressed()
    {        
        if (_movementSkills?.Count >= 1)
        {            
            var moveMentSkill = _movementSkills[0];
            ISkillData moveMentSkillData = moveMentSkill.Item2 as ISkillData;

            if (_currentFirstMovementSkillCoolTime?.Value > 0)
            {
                return;
            }
            else {
                if (moveMentSkillData.CoolTime != null) {
                    _currentFirstMovementSkillCoolTime.Value = _firstMovementSkillCoolTime;
                    if (_firstMovementSkillCoolTimeCoroutine == null)
                    {
                        _firstMovementSkillCoolTimeCoroutine = 
                        StartCoroutine(
                            SkillCoolDown(
                                _currentFirstMovementSkillCoolTime, () => {_firstMovementSkillCoolTimeCoroutine = null;}
                            )
                        );
                    }
                }
                _firstMoveSkill = moveMentSkill.Item1.ToString();
                if (_actionFsm.CurrentState != moveMentSkill.Item1)
                    SetActionStateServer(_firstMoveSkill, this);

                return;
            }
        }
    }

    public void OnFirstMoveSkillReleased()
    {
        if (_movementSkills?.Count >= 1)
        {
            if (_actionFsm.CurrentState == _movementSkills[0].Item1)
            {
                SetActionStateServer("Idle", this);
            }
        }
    }

    public void OnSecondMoveSkillPressed()
    {
        if (_movementSkills?.Count >= 2)
        {
            var moveMentSkill = _movementSkills[1];
            ISkillData moveMentSkillData = moveMentSkill.Item2 as ISkillData;

            if (_currentSecondMovementSkillCoolTime?.Value > 0)
            {
                return;
            }
            else
            {
                if (moveMentSkillData.CoolTime != null)
                {
                    _currentSecondMovementSkillCoolTime.Value = _secondMovementSkillCoolTime;
                    if (_secondMovementSkillCoolTimeCoroutine == null)
                    {
                        _secondMovementSkillCoolTimeCoroutine =
                        StartCoroutine(
                            SkillCoolDown(
                                _currentSecondMovementSkillCoolTime, () => { _secondMovementSkillCoolTimeCoroutine = null; }
                            )
                        );
                    }
                }
                _secondMoveSkill = moveMentSkill.Item1.ToString();
                if (_actionFsm.CurrentState != moveMentSkill.Item1)
                    SetActionStateServer(_secondMoveSkill, this);

                return;
            }
        }
    }

    public void OnSecondMoveSkillReleased()
    {
        if (_movementSkills?.Count >= 2)
        {
            if (_actionFsm.CurrentState == _movementSkills[1].Item1)
            {
                SetActionStateServer("Idle", this);
            }
        }
    }


    IEnumerator SkillCoolDown(ObservableFloat value, Action onComplete)
    {
        while (value.Value >= 0)
        {
            value.Value -= Time.deltaTime;
            Debug.Log("스킬 쿨타임 : " + value.Value);
            yield return null;
        }

        onComplete?.Invoke();
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
                controllerToApply = swordAnimatorController;
                break;
            case WeaponType.Gun:
                controllerToApply = gunAnimatorController;
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
    public bool IsIdle(out Action turningStep, float accumulatedYaw)
    {
        if (_actionFsm.CurrentState == ActionState.Dead)
        {
            turningStep = () => { Animator.SetLayerWeight(1, 0f); };
            return true;
        }
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
        if (stat == StatType.MaxHp)
        {
            if (CurrentHp.Value >= baseMaxHp.Value)
            {
                CurrentHp.Value = baseMaxHp.Value;
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
            if (CurrentHp.Value >= baseMaxHp.Value)
            {
                CurrentHp.Value = baseMaxHp.Value;
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
            if (CurrentHp.Value >= baseMaxHp.Value)
            {
                CurrentHp.Value = baseMaxHp.Value;
            }
            Debug.Log($"버프 적용 현재 체력{CurrentHp}");
        }
    }

    public void ChangeMovementSpeed(float value)
    {
        this.Animator.SetFloat(MoveSpeedHash, value);
    }


    //구매 내역 에 따른 스탯 분배 메소드 필요
    public void DecorateStatByPlayerItems()
    {
        if (_playerItems == null) return;
        var AR = _playerItems.count_AR;
        var mv = _playerItems.count_MV;
        var hp = _playerItems.count_HP;
        var jp = _playerItems.count_JP;

        AddStatDecorates(StatType.MaxHp, hp, maxHpIncreaseAmount);
        AddStatDecorates(StatType.Defence, AR, defenceIncreaseAmount);
        AddStatDecorates(StatType.MoveSpeed, mv, moveSpeedIncreaseAmount);
        AddStatDecorates(StatType.JumpPower, jp, jumpPowerIncreaseAmount);
    }

    public void AddStatDecorates(StatType statype, int count, float amount)
    {
        for (int i = 0; i < count; i++)
        {
            AddStatDecorate(statype, amount);
        }
    }


    //구매 내역에 따른 Passive 생성, 스킬 생성도 필요


    #endregion


    // 디버깅 용 Ray
    private void DrawRay()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * 5f, Color.red);
    }

    #region 옵저버
    public void WhenStatChanged((float, string) data)
    {
        Debug.Log($"{data.Item2}가 {data.Item1}로 변경되었습니다.");
        switch (data.Item2)
        {
            case "baseMoveSpeed":
                ChangeMovementSpeed(data.Item1);
                break;
        }
    }
    #endregion

    #region 캐릭터 초기화
    //timer가 끝나면 ResetCharacter를 호출 후 ReInit을 호출
    /// <summary>
    /// 기존 캐릭터에 적용되어 있던 스텟,스킬,패시브 제거
    /// </summary>
    public void ResetCharacter()
    {
        // 추가 생성한 스킬, 패시브 제거
        if (_movementSkills?.Count > 0)
        {
            foreach (var skill in _movementSkills)
            {
                Destroy((UnityEngine.Object)skill.Item2);
                ActionFsm.RemoveState(skill.Item1);
            }
        }
        _movementSkills?.Clear();

        if (_weaponSkills?.Count > 0)
        {
            foreach (var skill in _weaponSkills)
            {
                Destroy((UnityEngine.Object)skill.Item2);
                ActionFsm.RemoveState(skill.Item1);
            }
        }
        _weaponSkills?.Clear();

        if (_passiveList?.Count > 0)
        {
            foreach (var passive in _passiveList)
            {
                Destroy((UnityEngine.Object)passive);
            }
        }
        _passiveList?.Clear();

        //스텟 초기화
        foreach (var stat in statDictionary)
        {
            RemoveStatAllDecorate(stat.Key);
            //옵저버는 이미 보고 있는 곳이 있을 수 있으니 지우지 말기
            //stat.Value.RemoveObserver(this);
        }

        _firstWeaponSkillCoolTime = 0;
        _secondWeaponSkillCoolTime = 0;
        _firstMovementSkillCoolTime = 0;
        _secondMovementSkillCoolTime = 0;

        _currentFirstWeaponSkillCoolTime = null;
        _currentSecondWeaponSkillCoolTime = null;
        _currentFirstMovementSkillCoolTime = null;
        _currentSecondMovementSkillCoolTime = null;

        _firstMovementSkillCoolTimeCoroutine = null;
        _secondMovementSkillCoolTimeCoroutine = null;
        _firstWeaponSkillCoolTimeCoroutine = null;
        _secondWeaponSkillCoolTimeCoroutine = null;

        _purchaseManager = null;
    }


    /// <summary>
    /// ResetCharacter 후 새로운 구매내역이 생길 때 Init 대신 ReInit호출
    /// </summary>
    public void ReInit()
    {
        // InputManager 재구독 
        this.transform.GetComponent<InputManager>()?.Register(this);

        // 구매내역 할당
        _playerItems = PurchaseManager.PurchasedPlayerItems?.DeepCopy();

        //카메라 설정 (아마 기존에 이미 들어가 있어서 없어도 괜찮을 듯)
        //_cameraController = Camera.main?.GetComponent<CameraController>();
        // 멀티플레이에서 메인카메라를 사용한다면 둘의 카메라가 겹칠 수 있기 때문에 개별카메라로 변경하였습니다.
        cameraController.ResetCamera(head);

        //상태 변경
        ActionFsm?.ChangeState(defaultState, this);
        MovementFsm?.ChangeState(defaultState, this);
        PostureFsm?.ChangeState(defaultState, this);

        // 무기 설정 
        if (_playerItems?.weapon_Type == 1)
        {
            _playerWeapon.WeaponType = WeaponType.Gun;
        }
        else if (_playerItems?.weapon_Type == 2) { 
            _playerWeapon.WeaponType = WeaponType.Sword;
        }

        EquipWeapon(_playerWeapon);


        // 스텟 + currentHp 옵저버 등록 
        foreach (var stat in statDictionary)
        {
            stat.Value.AddObserver(this);
        }

        currentHp.AddObserver(this);

        // 구매내역에 따른 스텟 분배
        DecorateStatByPlayerItems();

        //구매 목록에 따른 패시브 적용
        _passiveFactory?.CreatePassive(this, _playerItems?.Skills[2]);

        if (PassiveList != null)
        {
            foreach (var passive in PassiveList)
            {
                passive.ApplyPassive(this);
            }
        }

        //스킬 목록 적용
        _weaponSkills = ActionFsm?.AddSkillState(this, _playerItems?.Skills[1], 1);
        _movementSkills = ActionFsm?.AddSkillState(this, _playerItems?.Skills[0], 0);


        // 스킬 쿨타임 받아오기

        switch (_weaponSkills?.Count) {
            case 1:
                _firstWeaponSkillCoolTime = _weaponSkills[0].Item2 is ISkillData firstWeaponSkill && firstWeaponSkill.CoolTime?.Value != null ? firstWeaponSkill.CoolTime.Value : 0f;
                break;
            case 2:
                _secondWeaponSkillCoolTime = _weaponSkills[1].Item2 is ISkillData secondWeaponSkill && secondWeaponSkill.CoolTime?.Value != null ? secondWeaponSkill.CoolTime.Value : 0f;
                break;
        }

        switch (_movementSkills?.Count) {
            case 1:
                _firstMovementSkillCoolTime =  _movementSkills[0].Item2 is ISkillData firstMovementSkill && firstMovementSkill.CoolTime?.Value != null ? firstMovementSkill.CoolTime.Value : 0f;
                break;
            case 2:
                _secondMovementSkillCoolTime = _movementSkills[1].Item2 is ISkillData secondMovementSkill && secondMovementSkill.CoolTime?.Value != null ? secondMovementSkill.CoolTime.Value : 0f;
                break;
        }     

        _currentFirstWeaponSkillCoolTime = new ObservableFloat(0, "currentFirstWeaponSkillCoolTime");
        _currentSecondWeaponSkillCoolTime = new ObservableFloat(0, "currentSecondWeaponSkillCoolTime");
        _currentFirstMovementSkillCoolTime = new ObservableFloat(0, "currentFirstMovementSkillCoolTime");
        _currentSecondMovementSkillCoolTime = new ObservableFloat(0, "currentSecondMovementSkillCoolTime");

        _isDead = false;

        //풀피 만들어주기
        CurrentHp.Value = baseMaxHp.Value;
        //모든 _playerItems의 적용이 끝났다면 PurchaseManager의 값 초기화
        PurchaseManager.ResetPurchasedPlayerItems();
    }

    //public void Init()
    //{
    //    //구매내역 가져오기
    //    _playerItems = PurchaseManager.PurchasedPlayerItems?.DeepCopy();

    //    // InputManager 구독 
    //    InputManager.instance.Register(this);

    //    // 카메라 설정
    //    _cameraController = Camera.main.GetComponent<CameraController>();
    //    // _cameraController.SetTarget(transform);
    //    // _cameraController.SetSpineTarget(rotationTarget);
    //    // _cameraController.IsIdle = IsIdle;

    //    // 무기 설정 
    //    EquipWeapon(_playerWeapon);

    //    _movementFsm.Run(this);
    //    _postureFsm.Run(this);
    //    _actionFsm.Run(this);

    //    // 구매내역에 따른 스텟 분배
    //    DecorateStatByPlayerItems();


    //    // 스텟 + currentHp 옵저버 등록 
    //    foreach (var stat in statDictionary)
    //    {
    //        stat.Value.AddObserver(this);
    //    }

    //    currentHp.AddObserver(this);


    //    //구매 목록에 따른 패시브 적용
    //    //임시, 구매내역이라 치고 작성
    //    //var myArray2 = new (int, string)[] { (1, "HpRegenerationPassive") };
    //    _passiveFactory.CreatePassive(this, _playerItems.Skills[2]);
    //    foreach (var passive in PassiveList)
    //    {
    //        passive.ApplyPassive(this);
    //    }
    //    //스킬 목록 적용
    //    //플레이어의 ActionFsm 내에 상태를 넣어야 함
    //    //AddSkillState에 넣을 스킬 목록을 집어넣으면 알아서 State가 생성됨
    //    //단 ActionState과 SkillFactory에 등록해두어야 추가 가능
    //    _weaponSkills = ActionFsm.AddSkillState(this, _playerItems.Skills[1], 1);
    //    _movementSkills = ActionFsm.AddSkillState(this, _playerItems.Skills[0], 0);
    //    //var myArray = new (int, string)[] { (1, "MovementSkills")  };
    //    //var myArray1 = new (int, string)[] { (1, "MovementSkills") , (1, "MovementSkills") };
    //    //_weaponSkills = ActionFsm.AddSkillState(myArray);
    //    //_movementSkills = ActionFsm.AddSkillState(myArray1);


    //    _isDead = false;

    //    //풀피 만들어주기
    //    CurrentHp.Value = baseMaxHp.Value;
    //    //모든 _playerItems의 적용이 끝났다면 PurchaseManager의 값 초기화
    //    PurchaseManager.ResetPurchasedPlayerItems();
    //}

    #endregion

}
