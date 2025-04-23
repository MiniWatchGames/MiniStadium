using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public enum StateType { 
    Action,
    Posture,
    Move,
}

public static class StateFactory
{    
    public static List<(T, IPlayerState)> CreateStates<T>(this PlayerFSM<T> fsm, IWeaponAnimationStrategy _aniStrategy) where T : Enum
    {
        if (typeof(T) == typeof(ActionState))
        {
            var list = (fsm as PlayerFSM<ActionState>).CreateStates(_aniStrategy);
            return list.Select(tuple => ((T)(object)tuple.Item1, tuple.Item2)).ToList();
        }
        else if (typeof(T) == typeof(MovementState))
        {
            var list = (fsm as PlayerFSM<MovementState>).CreateStates(_aniStrategy);
            return list.Select(tuple => ((T)(object)tuple.Item1, tuple.Item2)).ToList();
        }
        else if (typeof(T) == typeof(PostureState)) {

            var list = (fsm as PlayerFSM<PostureState>).CreateStates(_aniStrategy);
            return list.Select(tuple => ((T)(object)tuple.Item1, tuple.Item2)).ToList();
        }
        return null;
    }

    public static List<(ActionState, IPlayerState)> CreateStates(this PlayerFSM<ActionState> fsm, IWeaponAnimationStrategy _aniStrategy) => new()
    {
        (ActionState.Idle, new ActionIdleState(_aniStrategy)),
        (ActionState.Attack, new AttackState(_aniStrategy)),
        (ActionState.Hit, new HitState(_aniStrategy)),
        (ActionState.Dead, new DeadState(_aniStrategy)),
        (ActionState.MovementSkills, new MovementSkillsState(_aniStrategy)),
        (ActionState.WeaponSkills, new WeaponSkillsState(_aniStrategy))
    };

    public static List<(MovementState, IPlayerState)> CreateStates(this PlayerFSM<MovementState> fsm, IWeaponAnimationStrategy _aniStrategy) => new()
    {
        (MovementState.Walk, new WalkState(_aniStrategy)),
        (MovementState.Jump, new JumpState(_aniStrategy)),
        (MovementState.Idle, new MovementIdleState(_aniStrategy))
    }; 
    public static List<(PostureState, IPlayerState)> CreateStates(this PlayerFSM<PostureState> fsm, IWeaponAnimationStrategy _aniStrategy) => new()
    {
        (PostureState.Idle, new PostureIdleState(_aniStrategy)),
        (PostureState.Crouch, new CrouchState(_aniStrategy))
    };
}

/// <summary>
/// FSM은 상태머신을 구현하기 위한 클래스입니다.
/// </summary>
/// <typeparam name="T">character의 state를 각각 나눠둔 enum을 받음</typeparam>
public class PlayerFSM<T> where T : Enum
{
    private string _defaultState;
    private IPlayerState _currentState;
    private Dictionary<Enum, IPlayerState> _states = new();
    private StateType _stateType;
    private PlayerWeapon _playerWeapon;
    private PlayerAnimationStrategyFactory _playerAnimationStrategyFactory;
    private IWeaponAnimationStrategy _aniStrategy;
    public T CurrentState { get; private set; }


    /// <summary>
    /// FSM의 생성자입니다.
    /// </summary>
    /// <param name="value"></param>
    public PlayerFSM(StateType value, PlayerWeapon playerWeapon, string defaultState)
    {
        _defaultState = defaultState;
        _stateType = value;
        _playerWeapon = playerWeapon;
        _playerAnimationStrategyFactory = new PlayerAnimationStrategyFactory();
        _aniStrategy = _playerAnimationStrategyFactory.CreateStrategy(playerWeapon);
    }  

    public void Run(PlayerController playerController) 
    {
        List<(T, IPlayerState)> list = this.CreateStates(_aniStrategy);
        foreach (var state in list)
        {
            AddState(state.Item1, state.Item2);
        }

        ChangeState(_defaultState, playerController);
    }

    public void AddState(T stateType, IPlayerState state)
    {
        if (_states.ContainsKey(stateType))
        {
            Debug.LogError($"State {stateType} already exists.");
            return;
        }
        _states.Add(stateType, state);
    }

    public void ChangeState(string stateName, PlayerController playerController) {
        T stateType = (T)(object)Enum.Parse(typeof(T), stateName);
        _currentState?.Exit();
        if (!_states.TryGetValue(stateType, out _currentState)) return;
        CurrentState = stateType; // 현재 상태 저장
        _currentState?.Enter(playerController);
    }

    public void CurrentStateUpdate()
    {
        _currentState?.Update();
    }

    /// <summary>
    /// 무기 변경시 새 전략으로 바꾸기위해 호출해줘야 함
    /// </summary>
    public void ChangeWeapon(PlayerWeapon playerWeapon, PlayerController playerController) {
        _playerWeapon = playerWeapon;
        _states.Clear();
        _states = null;
        _aniStrategy = _playerAnimationStrategyFactory.CreateStrategy(_playerWeapon);
        ChangeState(_defaultState, playerController);
    }
   
}
