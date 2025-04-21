using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RepairShopSkill : MonoBehaviour
{
    [SerializeField] private GameObject skillSlot;
    [SerializeField] private Transform moveSkillT;
    [SerializeField] private Transform weaponSkillT;
    [SerializeField] private Transform passiveSkillT;
    private List<RepairShopSkillSlot>[] BOskillList;
    private BuyableObject_Skill[] moveSkills;
    private BuyableObject_Skill[] weaponSkills;
    private BuyableObject_Skill[] passiveSkills;
    
    void Start()
    {
        GenerateSkillUI();
    }
    
    void LoadSkillList()
    {
        BOskillList = new List<RepairShopSkillSlot>[3];
        for (int i = 0; i < BOskillList.Length; i++)
            BOskillList[i] = new List<RepairShopSkillSlot>();
        
        moveSkills = Resources.LoadAll<BuyableObject_Skill>($"ScriptableObejct/Skill/0MoveSkill");
        weaponSkills = Resources.LoadAll<BuyableObject_Skill>($"ScriptableObejct/Skill/1WeaponSkill");
        passiveSkills = Resources.LoadAll<BuyableObject_Skill>($"ScriptableObejct/Skill/2PassiveSkill");
    }
    
    void GenerateSkillUI()
    {
        LoadSkillList();
    
        Transform[] targets = { moveSkillT, weaponSkillT, passiveSkillT };
        BuyableObject_Skill[][] skillGroups = { moveSkills, weaponSkills, passiveSkills };

        for (int i = 0; i < 3; i++)
        {
            foreach (var bo in skillGroups[i])
            {
                var slot = Instantiate(skillSlot, targets[i]);
                var script = slot.GetComponent<RepairShopSkillSlot>();
                script.Init(bo, this);
                BOskillList[i].Add(script);
            }
        }
    }

    void BuyingSkill()
    {
        
    }

    void SkillShopReSet()
    {
        
    }
}
