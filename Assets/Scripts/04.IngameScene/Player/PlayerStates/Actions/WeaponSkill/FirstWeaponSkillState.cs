using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstWeaponSkillState : PlayerActionState, ISkillData
{
    private static int aniName;
    public FirstWeaponSkillState() : base() { }
    
    private ObservableFloat _coolTime;
    private float _skillMount;
    public ObservableFloat CoolTime => _coolTime;
    public ObservableFloat NeedPressTime { get; }
    public ObservableFloat PressTime { get; }
    public bool IsNeedPresse { get; }
    public float SkillMount => _skillMount;
    
    private void Awake()
    {
        _coolTime = new ObservableFloat(10, "_coolTime");
        _skillMount = 30;
    }
    
    public override void Enter(PlayerController playerController)
    {
        base.Enter(playerController);
        _playerController.CombatManager.StartFirstSkill();
    }
    public override void Exit()
    {
        _playerController.CombatManager.EndFirstSkill();
        base.Exit();
    }

    public void ResetSkill()
    {
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        _playerController.CombatManager.UpdateFirstSkill();
        if (_playerController.CombatManager.IsFirstSkillComplete())
        {
            _playerController.SetActionState("Idle");
        }
    }
}