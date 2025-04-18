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
        playerController.Animator.SetBool(IsMoving, false);
        
        // 움직임 관련 파라미터 초기화
        playerController.Animator.SetFloat("MoveX", 0f);
        playerController.Animator.SetFloat("MoveZ", 0f);
        
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