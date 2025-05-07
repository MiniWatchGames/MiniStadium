using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerMovementState, IStatObserver
{
    private Vector2 _smoothedInput = Vector2.zero;
    private float _smoothingSpeed = 10f;
    public float footstepInterval = 0.35f;
    float timer = 0f;
    private static int aniName;
    public WalkState() : base() { }
    //public WalkState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("Walk"));
    //}
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController); 
        float moveSpeed = _playerController.BaseMoveSpeed.Value;
        footstepInterval = 0.35f * (1.0f / moveSpeed);
        _playerController.BaseMoveSpeed.AddObserver(this);
        _playerController.Animator.SetBool(IsMoving, true);
    }
    public override void Exit()
    {
        base.Exit();    
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
        _smoothedInput = Vector2.Lerp(_smoothedInput, _playerController.CurrentMoveInput, Time.deltaTime * _smoothingSpeed);
        timer += Time.deltaTime;
        if (timer >= footstepInterval)
        {
            timer = 0f;
            _playerController.PlayFootstep();
        }
        // 매우 작은 값은 0으로 처리
        if (_smoothedInput.magnitude < 0.01f)
            _smoothedInput = Vector2.zero;

        // 애니메이터에 스무딩된 값 전달
        _playerController.Animator.SetFloat("MoveX", _smoothedInput.x);
        _playerController.Animator.SetFloat("MoveZ", _smoothedInput.y);
        
    }

    public void WhenStatChanged((float, string) data)
    {
        footstepInterval = 0.35f * (1.0f /data.Item1);
    }
}