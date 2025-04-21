using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerMovementState
{
    private static int aniName;
    public JumpState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    {
        aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Jump"));
    }
    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);

        _playerController.Animator.SetTrigger(Jump);
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