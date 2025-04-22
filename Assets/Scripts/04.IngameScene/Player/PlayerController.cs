using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IInputEvents
{
    [SerializeField] private LayerMask groundLayer;
    
    private CharacterController _characterController;
    private CameraController _cameraController;
    private const float _gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
    private float _jumpSpeed = 0.5f;
    
    // input값 저장, 전달 
    private Vector2 _currentMoveInput;
    public Vector2 CurrentMoveInput => _currentMoveInput;
    private float _lastInputTime;
    private float _inputBufferTime = 0.1f; // 100ms의 버퍼 타임
    
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
    
    [Header("Combat")]
    [SerializeField] private CombatManager combatManager;
    public CombatManager CombatManager { get => combatManager; }

    // --------
    // 애니메이션 관련 
    [SerializeField] private RuntimeAnimatorController swordController;
    [SerializeField] private RuntimeAnimatorController gunController;
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

        // 무기 설정 
        EquipWeapon(_playerWeapon);
        
        _movementFsm.Run(this);
        _postureFsm.Run(this);
        _actionFsm.Run(this);
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
            _velocity.y = Mathf.Sqrt(_jumpSpeed * -2f * _gravity);
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



    // 디버깅 용 Ray
    private void DrawRay()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * 5f, Color.red);
    }

}
