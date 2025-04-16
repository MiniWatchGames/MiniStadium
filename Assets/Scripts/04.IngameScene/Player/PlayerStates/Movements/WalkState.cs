using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerMovementState
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static int aniName;
    public WalkState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    {
        aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Walk"));
    }
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        _playerController.Animator.SetBool(IsMoving, true);
    }
    public override void Exit()
    {
        _playerController.Animator.SetBool(IsMoving, false);
        base.Exit();    
    }
    public override void Update()
    {
        Vector2 input = _playerController.CurrentMoveInput;
        _playerController.Animator.SetFloat("MoveX", input.x);
        _playerController.Animator.SetFloat("MoveZ", input.y);
        base.Update();    
    }
}