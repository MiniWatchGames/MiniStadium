using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPostureState : IPlayerState
{
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

    public PlayerPostureState(IWeaponAnimationStrategy iWeaponAnimationStrategy)
    {
        _aniStrategy = iWeaponAnimationStrategy;
    }
}
