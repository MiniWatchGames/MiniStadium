using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerMovementState
{
    private static int aniName;
    public JumpState() : base() { }
    //public JumpState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Jump"));
    //}
    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);
        _playerController.LandSound = () => {
            _playerController.Playland();
            _playerController.LandSound = null;
        };
        _playerController.Animator.SetTrigger(Jump);
        _playerController.PlayFirstJump();
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