using System;
using System.Collections.Generic;

public class PlayerItems
{
    // weapon
    // type 0=None 1=Ranged 2=Melee
    public int weapon_Type = -1; 
    public int weapon_ID = -1;
    public string weapon_Name = null;
    
    // status
    public int count_HP = 0;
    public int count_AR = 0;
    public int count_MV = 0;

    public (int, string)[][] Skills = new (int, string)[3][];
    //(0,"")일때 null
    // int= ID, string= name
    // [0][] = Movement skill
    // [1][] = Weapon skill
    // [2][] = Passive skill

    public PlayerItems DeepCopy()
    {
        PlayerItems clone = (PlayerItems)this.MemberwiseClone();

        // Skills 배열의 깊은 복사 수행
        if (Skills != null)
        {
            clone.Skills = new (int, string)[3][];

            for (int i = 0; i < 3; i++)
            {
                if (Skills[i] != null)
                {
                    clone.Skills[i] = new (int, string)[Skills[i].Length];
                    for(int j = 0; j < Skills[i].Length; j++)
                    {
                        clone.Skills[i][j] = Skills[i][j];
                    }
                }
            }
        }

        return clone;
    }
}
