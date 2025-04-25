using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIdleState : PlayerActionState
{
    private static int aniName;
    public ActionIdleState() : base() { }
    //public ActionIdleState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("ActionIdle"));
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