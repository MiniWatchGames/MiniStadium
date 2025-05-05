using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RunSkillState : PlayerActionState ,ISkillData
{
    //이동 스킬 블루 프린트입니다
    private static int aniName;
    public RunSkillState() : base() { }

    private bool _isNeedPresse;
    private float _skillMount;
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
        _playerController.AddStatDecorate(StatType.MoveSpeed, _skillMount);

    }
    public override void Exit()
    {
        _playerController.RemoveStatDecorate(StatType.MoveSpeed);
        base.Exit();
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
    }
}