using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : PlayerPostureState
{
    private static int aniName;

    public CrouchState() : base()
    {
    }

    //public CrouchState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{

    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Crouch"));

    //}
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        _playerController.Animator.SetBool(IsCrouch, true);
    }
    public override void Exit()
    {
        _playerController.Animator.SetBool(IsCrouch, false);
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
}