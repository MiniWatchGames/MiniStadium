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
    public int count_JP = 0;

    public (int, string)[][] Skills = new (int, string)[3][];
    //(0,"")�϶� null
    // int= ID, string= name
    // [0][] = Movement skill
    // [1][] = Weapon skill
    // [2][] = Passive skill

}
