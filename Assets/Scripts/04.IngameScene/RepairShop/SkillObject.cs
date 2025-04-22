using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 1)]
public class SkillObject : ScriptableObject
{
    public string skillName;
    
    public BuyableObject buyableObject;
    
    public string description;
    
    public int damage;
    
    public int cooldown;
    
    public int range;
    
    public int duration;
}

