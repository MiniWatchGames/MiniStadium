using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerActionState
{
    private static int aniName;

    public AttackState() : base() { }
    //public AttackState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Attack"));
    //}
    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);
        // todo : Set Animator Parameter
    }
    public override void Exit()
    {
        _playerController.CombatManager.EndAttack();
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        _playerController.CombatManager.UpdateAttack();
    }
}