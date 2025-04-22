using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct skillAtribute
{
    public string name;
    public float value;
}
[System.Serializable]
public struct skill
{
    public string skillName;
    
    public Sprite icon;
    
    public string description;
    
    public float coolTime;
    
    public float duration;
    
    public float damage;

    public float price;
    
    public RepairShopEnums.SkillType skillType;
    
    public List<skillAtribute> skillAtributes;
}

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill", order = 1)]
public class SkillObject : ScriptableObject
{
    public List<skill> skills = new List<skill>();
}

