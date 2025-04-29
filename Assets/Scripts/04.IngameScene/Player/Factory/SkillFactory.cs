using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    /// <summary>
    /// 고른 스킬에 맞는 스킬들을 생성해서 반환  
    /// </summary>
    /// <param name="skills"> int 스킬번호, string 스킬명</param>
    /// <param name="SkillType"> 0:Movement, 1:Weapon</param>
    /// <returns></returns>
    public static List<(ActionState, IPlayerState)> CreateStates(this PlayerFSM<ActionState> fsm, (int,string)[] skills, int SkillType)
    {
        List<(ActionState, IPlayerState)> states = new List<(ActionState, IPlayerState)>();
        switch(SkillType)
        {
            case 0: //Movement
                foreach (var skillName in skills)
                {
                    switch (skillName.Item1)
                    {
                        //"MovementSkills"
                        case 1:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 3:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 2:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                    }
                }
                return states;
            case 1: //Weapon
                foreach (var skillName in skills)
                {
                    switch (skillName.Item1)
                    {
                        //"MovementSkills"
                        case 1:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 2:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 3:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 4:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 5:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                        case 6:
                            states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                            break;
                    }
                }
                return states;
        }
        return null;
  
    }
}
