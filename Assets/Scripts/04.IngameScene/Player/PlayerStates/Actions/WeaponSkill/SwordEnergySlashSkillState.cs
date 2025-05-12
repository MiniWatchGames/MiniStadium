using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEnergySlashSkillState : PlayerActionState, ISkillData
{
    
    public ObservableFloat CoolTime { get; }
    public bool IsNeedPresse { get; }
    public ObservableFloat NeedPressTime { get; }
    public ObservableFloat PressTime { get; }
    public float SkillMount { get; }
    public SwordEnergySlashSkillState(PlayerController playerController)
    {
        _playerController = playerController;
        // CoolTime = new ObservableFloat(0);
        // IsNeedPresse = false;
        // NeedPressTime = new ObservableFloat(0);
        // PressTime = new ObservableFloat(0);
        // SkillMount = 1;
    }
    public void ResetSkill()
    {
        
    }
    public void Enter(PlayerController playerController)
    {
        //playerActionState에 있는 playerController에 값을 넣기 위함
       base.Enter(playerController);
       
       
    }

    public void StateUpdate()
    {
        
    }

    public void Exit()
    {
        
    }

    
}
