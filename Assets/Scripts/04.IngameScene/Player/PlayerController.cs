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
    [SerializeField] private Transform spine; // 캐릭터의 상체(척추) 본
    
    private CharacterController _characterController;
    private CameraController _cameraController;
    private const float _gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
    private float jumpSpeed = 0.5f;
    
    // input값 저장, 전달 
    private Vector2 _currentMoveInput;
    public Vector2 CurrentMoveInput => _currentMoveInput;
    
    // --------
    // 상태 관련
    [Header("FSM")]
    private PlayerFSM<MovementState> _movementFsm;
    private PlayerFSM<PostureState> _postureFsm;
    private PlayerFSM<ActionState> _actionFsm;
    [SerializeField] private string defaultState;
    
    // --------
    // 카메라 관련
    [Header("Camera")]
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    private float _yaw = 0f;
    private float _pitch = 0f;
    
    [Header("Weapon")]
    PlayerWeapon _playerWeapon;

    public Animator Animator { get; private set; }
    public bool IsGrounded
    {
        get
        {
            return GetDistanceToGround() < 0.2f;
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
    }

    private void Init()
    {
        // 카메라 설정
        _cameraController = Camera.main.GetComponent<CameraController>();
        _cameraController.SetTarget(transform);
        
        _movementFsm.Run(this);
        _postureFsm.Run(this);
        _actionFsm.Run(this);

        SetMovementState("Idle");
        SetPostureState("Idle");
        SetActionState("Idle");
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

    private void OnAnimatorMove()
    {
        Vector3 movePosition;

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
        // 이동 
        if (input != Vector2.zero)
        {
            if (_movementFsm.CurrentState != MovementState.Walk)
            {

                SetMovementState("Walk");
            }
            _currentMoveInput = input;
        }
        else
        {
            if (_movementFsm.CurrentState != MovementState.Idle)
            {
                SetMovementState("Idle");
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
    
        // 캐릭터 Y축 회전 적용 (수평 회전)
        transform.rotation = Quaternion.Euler(0, _yaw, 0);
    
        // 카메라 Pitch 업데이트
        // 수직 회전의 일부를 상체에 적용 (전체 피치의 일정 비율)
        _cameraController.SetPitch(_pitch);
    }

    public void OnJumpPressed()
    {
        // 점프 
        // todo : 착지 타이밍과 상태 전환 타이밍 동기화 필요 
        SetMovementState("Jump");
        _velocity.y = Mathf.Sqrt(jumpSpeed * -2f * _gravity);
    }

    public void OnFirePressed()
    {
        // 공격
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
        if (Physics.Raycast(transform.position + new Vector3(0,0.1f,0), 
                Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }    
}
