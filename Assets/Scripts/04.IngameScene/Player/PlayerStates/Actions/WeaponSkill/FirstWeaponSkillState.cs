using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstWeaponSkillState : PlayerActionState, ISkillData
{
    private static int aniName;
    public FirstWeaponSkillState() : base() { }
    
    public ObservableFloat CoolTime { get; }
    public ObservableFloat NeedPressTime { get; }
    public ObservableFloat PressTime { get; }
    public bool IsNeedPresse { get; }
    public float SkillMount { get; }
    
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