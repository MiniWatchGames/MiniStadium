using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSkillsState : PlayerActionState
{
    //이동 스킬 블루 프린트입니다
    private static int aniName;
    public MovementSkillsState() : base() { }
    //public MovementSkillsState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("MovementSkills"));
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