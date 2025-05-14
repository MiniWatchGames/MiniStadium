using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class SkillFactory
{
    /// <summary>
    /// 고른 스킬에 맞는 스킬들을 생성해서 반환  
    /// </summary>
    /// <param name="skills"> int 스킬번호, string 스킬명</param>
    /// <param name="SkillType"> 0:Movement, 1:Weapon</param>
    /// <returns></returns>
    public static List<(ActionState, IPlayerState)> CreateStates(this PlayerFSM<ActionState> fsm, PlayerController target, (int,string)[] skills, int SkillType)
    {
        if (skills is null) return null;
        List<(ActionState, IPlayerState)> states = new List<(ActionState, IPlayerState)>();

        switch (SkillType)
        {
            case 0: //Movement
                foreach (var skillName in skills)
                {
                    IPlayerState skill = null;
                    switch (skillName.Item1)
                    {
                        //"MovementSkills"
                        case 1:
                            skill = target.AddComponent<RunSkillState>();
                            states.Add(new(ActionState.RunSkill, skill));
                            break;
                        case 2:
                            skill = target.AddComponent<DoubleJumpSkillState>();
                            states.Add(new(ActionState.DoubleJumpSkill, skill));
                            break;
                        case 3:
                            skill = target.AddComponent<TeleportSkillState>();
                            states.Add(new(ActionState.TeleportSkill, skill));
                            break;
                    }
                }
                return states;
            case 1: //Weapon
                foreach (var skillName in skills)
                {
                    IPlayerState skill = null;
                    switch (skillName.Item1)
                    {
                        // WeaponSkills
                        case 1:
                            skill = target.AddComponent<MissileState>();
                            states.Add(new(ActionState.Missile, skill));
                            break;
                        case 2:
                            skill = target.AddComponent<SmartMissileState>();
                            states.Add(new(ActionState.SmartMissile, skill));
                            break;
                        case 3:
                            skill = target.AddComponent<MovementSkillsState>();
                            states.Add(new(ActionState.MovementSkills, skill));
                            break;
                        case 4:
                            skill = target.AddComponent<FirstWeaponSkillState>();
                            states.Add(new(ActionState.FirstWeaponSkill, skill));
                            break;
                        case 5:
                            skill = target.AddComponent<MovementSkillsState>();
                            states.Add(new(ActionState.MovementSkills, skill));
                            break;
                        case 6:
                            skill = target.AddComponent<MovementSkillsState>();
                            states.Add(new(ActionState.MovementSkills, skill));
                            break;
                    }
                }
                return states;
        }
        return null;
  
    }
}
