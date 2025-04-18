using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostureIdleState : PlayerPostureState
{
    private static int aniName ; 
    public PostureIdleState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    {
        aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Posture"));
    }
    public override void Enter(PlayerController playerController)
    {        
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        _playerController.Animator.SetBool(IsCrouch, false);
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