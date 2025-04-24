using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    /// <summary>
    /// 고른 스킬에 맞는 스킬들을 생성해서 반환  
    /// </summary>
    /// <param name="_aniStrategy"></param>
    /// <param name="skillNames">스킬 이름들이 담긴 리스트</param>
    /// <returns></returns>
    public static List<(ActionState, IPlayerState)> CreateStates(this PlayerFSM<ActionState> fsm, List<string> skillNames)
    {
        List<(ActionState, IPlayerState)> states = new List<(ActionState, IPlayerState)>();
        foreach (var skillName in skillNames)
        {
            switch (skillName)
            {
                case "MovementSkills":
                    states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                    break;
            }
        }
        return states;
    }
}
