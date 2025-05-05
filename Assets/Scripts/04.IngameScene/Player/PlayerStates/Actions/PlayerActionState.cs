using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerActionState : MonoBehaviour, IPlayerState
{
    protected PlayerController _playerController;
   // protected IWeaponAnimationStrategy _aniStrategy;
    public virtual void Enter(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public virtual void Exit()
    {
    }

    public virtual void StateUpdate()
    {
    }

    //public PlayerActionState(IWeaponAnimationStrategy iWeaponAnimationStrategy) {
    //    _aniStrategy = iWeaponAnimationStrategy;
    //}
}
