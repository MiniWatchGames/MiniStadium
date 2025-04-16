using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : PlayerPostureState
{
    private static int aniName;

    public CrouchState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    {

        aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Crouch"));

    }
    public override void Enter(PlayerController playerController)
    {
        playerController.Animator.Play(aniName);
        base.Enter(playerController);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
}