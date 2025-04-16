using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementIdleState : PlayerMovementState
{
    private static int aniName;
    public MovementIdleState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    {
        aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("MovementIdle"));
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