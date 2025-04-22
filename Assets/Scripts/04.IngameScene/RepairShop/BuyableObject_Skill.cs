using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buyable_Skill", menuName = "ScriptableObjects/Skill", order = 1)]
public class BuyableObject_Skill : ScriptableObject
{
    public Sprite icon;
   
    public string name;
   
    public int price;
   
    public string description;
    
    // 0=MoveSkill 1=WeaponSkill 2=Passive
    public int skillType;
    
    // 0=All 1=Ranged 2=Melee
    public int weaponType;
    
}