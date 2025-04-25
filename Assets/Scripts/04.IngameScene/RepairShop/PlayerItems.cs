using System.Collections.Generic;

public class PlayerItems
{
    // weapon
    // type 0=None 1=Ranged 2=Melee
    public int weapon_Type; 
    public int weapon_Index;
    public string weapon_Name;
    
    // status
    public int count_HP = 0;
    public int count_AR = 0;
    public int count_MV = 0;
    
    public Dictionary<int, string>[] Skills = new Dictionary<int, string>[3];
    // int= type, string= name
    // type0 = Movement skill
    // type1 = Weapon skill
    // type2 = Passive skill
    
}
