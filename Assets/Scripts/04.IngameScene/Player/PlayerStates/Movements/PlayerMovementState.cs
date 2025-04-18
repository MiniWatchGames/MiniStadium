using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : IPlayerState
{
    protected static readonly int IsMoving = Animator.StringToHash("IsMoving");
    protected static readonly int Jump = Animator.StringToHash("Jump");

    protected PlayerController _playerController;
    protected IWeaponAnimationStrategy _aniStrategy;
    public virtual void Enter(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public virtual void Exit()
    {
        _playerController = null;
        _aniStrategy = null;
    }

    public virtual void Update()
    {
    }

    public PlayerMovementState(IWeaponAnimationStrategy iWeaponAnimationStrategy)
    {
        _aniStrategy = iWeaponAnimationStrategy;
    }
}
