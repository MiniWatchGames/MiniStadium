using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RepairShopEnums
{
    public enum WeaponType
    {
        Sword,
        Bow,
        Gun,
        Spear,
        Axe,
        Staff
    }
    public enum WeaponRange
    {
        Melee,
        Ranged,
        Magic
    }
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    public enum SkillType
    {
        Range,
        Melee,
        Movement,
        Buff,
        Debuff,
        Trap
    }
    public enum SkillTarget
    {
        Self,
        Enemy,
        Ally,
        Area
    }
    
}
