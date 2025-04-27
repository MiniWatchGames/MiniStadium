using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSkillsState : PlayerActionState
{
    // 무기 스킬 블루 프린트입니다
    private static int aniName;
    public WeaponSkillsState() : base() { }
    //public WeaponSkillsState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("WeaponSkills"));
    //}
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
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