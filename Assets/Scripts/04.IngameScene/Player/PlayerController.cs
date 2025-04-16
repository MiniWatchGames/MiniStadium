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
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsCrouch = Animator.StringToHash("isCrouch");
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform spine; // 캐릭터의 상체(척추) 본
    
    private CharacterController _characterController;
    private CameraController _cameraController;
    private const float _gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
    private float jumpSpeed = 0.5f;
    
    // --------
    // 상태 관련
    [Header("FSM")]
    private PlayerFSM<MovementState> _movementFsm;
    private PlayerFSM<PostureState> _postureFsm;
    private PlayerFSM<ActionState> _actionFsm;
    [SerializeField]
    private string defaultState;
    
    // --------
    // 카메라 관련
    [Header("Camera")]
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    private float _yaw = 0f;
    private float _pitch = 0f;
    
    [Header("Weapon")]
    Weapon _weapon;

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
        _weapon = GetComponent<Weapon>();
        
        _movementFsm = new PlayerFSM<MovementState>(StateType.Move, _weapon, defaultState);
        _postureFsm = new PlayerFSM<PostureState>(StateType.Posture, _weapon, defaultState);
        _actionFsm = new PlayerFSM<ActionState>(StateType.Action, _weapon, defaultState);
    }

    private void Start()
    {
        // InputManager 구독 
        InputManager.instance.Register(this);

        Init();
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
        // todo : 상태패턴으로 적용 
        if (input != Vector2.zero)
        {
            Animator.SetBool(IsMoving, true);
            Animator.SetFloat("MoveX", input.x);
            Animator.SetFloat("MoveZ", input.y);
        }
        else
        {
            Animator.SetBool(IsMoving, false);
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
        // todo : 상태패턴으로 적용 
        Animator.SetTrigger(Jump);
        _velocity.y = Mathf.Sqrt(jumpSpeed * -2f * _gravity);
    }

    public void OnFirePressed()
    {
        // 공격
    }

    public void OnCrouchPressed()
    {
        // 앉기 
        // todo : 상태패턴으로 적용 
        var isCrouch = Animator.GetBool(IsCrouch);
        Animator.SetBool(IsCrouch, !isCrouch);
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
