using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    /// <summary>
    /// �� ��ų�� �´� ��ų���� �����ؼ� ��ȯ  
    /// </summary>
    /// <param name="_aniStrategy"></param>
    /// <param name="skillNames">��ų �̸����� ��� ����Ʈ</param>
    /// <returns></returns>
    public static List<(ActionState, IPlayerState)> CreateStates(this PlayerFSM<ActionState> fsm, (int,string)[] skills)
    {
        List<(ActionState, IPlayerState)> states = new List<(ActionState, IPlayerState)>();
        foreach (var skillName in skills)
        {
            switch (skillName.Item1)
            {
                //"MovementSkills"
                case 0:
                    states.Add(new(ActionState.MovementSkills, new MovementSkillsState()));
                    break;
            }
        }
        return states;
    }
}
