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
        _playerController.CombatManager.StartAttack();
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
        // 공격 완료 확인 - 여기서 IdleState로 전환
        if (_playerController.CombatManager.IsAttackComplete())
        {
            _playerController.SetActionState("Idle");
        }
    }
}