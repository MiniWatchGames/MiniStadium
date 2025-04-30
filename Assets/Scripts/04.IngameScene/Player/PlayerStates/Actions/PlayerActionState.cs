using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillState {
    private float _coolTime;
    private int _canUseCount;
    private bool _isNeedPresse;
    private float _needPressTime;
    private float _skillMount;
}
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

    public virtual void Update()
    {
    }

    //public PlayerActionState(IWeaponAnimationStrategy iWeaponAnimationStrategy) {
    //    _aniStrategy = iWeaponAnimationStrategy;
    //}
}
