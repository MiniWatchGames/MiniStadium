using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerActionState
{
    private static int aniName;
    private static readonly int Dead = Animator.StringToHash("Dead");

    public DeadState() : base() { }
    //public DeadState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Dead"));
    //}
    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);
        
        InputManager.instance.Unregister(_playerController);
        
        _playerController.Animator.SetLayerWeight(1, 0f);
        _playerController.Animator.SetLayerWeight(2, 0f);
        _playerController.Animator.SetTrigger(Dead);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
    }
}