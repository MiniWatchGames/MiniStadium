using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static RepairShopEnums;

public class DoubleJumpSkillState : PlayerActionState, ISkillData
{
    public DoubleJumpSkillState() : base() { }

    private int Jump = Animator.StringToHash("Jump");
    private ObservableFloat _coolTime;
    private bool _isNeedPresse;
    private ObservableFloat _needPressTime;
    private float _skillMount;
    private bool _isAlreadyPressed;
    public ObservableFloat CoolTime => null;
    public ObservableFloat NeedPressTime => null;
    public ObservableFloat PressTime => null;

    public bool IsNeedPresse => _isNeedPresse;
    public float SkillMount => _skillMount;


    private void Awake()
    {
        _isNeedPresse = false;
        _skillMount = 1;
    }
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        //값 지정 필요 
        if (!_playerController.IsGrounded && !_isAlreadyPressed)
        {
            _isAlreadyPressed = true;
            _playerController.Jump(); 
            _playerController.PlaySecondJump();
            _playerController.Animator.SetTrigger(Jump);
        }

    }
    public override void Exit()
    {

        base.Exit();
    }

    //StateUpdate는 키입력이 있을 때만 호출이 되는데 
    //IsGrounded는 상시 관찰이 필요함 따라서
    //MonoBehaviour의 update를 사용함
    public void Update()
    {
        if (_playerController == null) return;

        if (_playerController.IsGrounded)
        {
            _isAlreadyPressed = false;
        }

    }
}