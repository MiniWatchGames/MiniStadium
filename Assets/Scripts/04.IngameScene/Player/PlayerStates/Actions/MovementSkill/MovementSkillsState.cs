using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MovementSkillsState : PlayerActionState, ISkillData
{
    //이동 스킬 블루 프린트입니다
    private static int aniName;
    public MovementSkillsState() : base() { }

    private ObservableFloat _coolTime;
    private ObservableFloat _pressTime;
    private ObservableFloat _needPressTime;
    private bool _isNeedPresse;
    private float _skillMount;
    public ObservableFloat CoolTime => _coolTime;

    public bool IsNeedPresse => _isNeedPresse;

    public ObservableFloat NeedPressTime => _needPressTime;

    public float SkillMount => _skillMount;

    public ObservableFloat PressTime => _pressTime;

    //public MovementSkillsState(IWeaponAnimationStrategy iWeaponAnimationStrategy) : base(iWeaponAnimationStrategy)
    //{
    //    aniName = Animator.StringToHash(_aniStrategy.GetAnimationName("MovementSkills"));
    //}
    public override void Enter(PlayerController playerController)
    {
        //playerController.Animator.Play(aniName);
        base.Enter(playerController);
        //값 지정 필요
        _coolTime = new ObservableFloat(0,"_coolTime");
        _needPressTime = new ObservableFloat(0, "_needPressTime");
        _pressTime = new ObservableFloat(0, "_pressTime");
        _skillMount = 0;
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
    }
}