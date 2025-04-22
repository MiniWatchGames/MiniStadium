using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RepairShopSkill : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopReceipt Receipt;
    [SerializeField] private GameObject skillSlot;
    [SerializeField] private Transform moveSkillT;
    [SerializeField] private Transform weaponSkillT;
    [SerializeField] private Transform passiveSkillT;
    private List<RepairShopSkillSlot>[] repairShopSkills;
    private BuyableObject_Skill[] BOmoveSkills;
    private BuyableObject_Skill[] BOweaponSkills;
    private BuyableObject_Skill[] BOpassiveSkills;
    public int selectedSkillIndex = -1;
    public int selectedSkillType = -1;
    public List<RepairShopSkillSlot> currentSkills;
    
    void Start()
    {
        GenerateSkillUI();
    }
    
    void LoadSkillList()
    {
        repairShopSkills = new List<RepairShopSkillSlot>[3];
        for (int i = 0; i < repairShopSkills.Length; i++)
            repairShopSkills[i] = new List<RepairShopSkillSlot>();
        
        BOmoveSkills = Resources.LoadAll<BuyableObject_Skill>($"ScriptableObejct/Skill/0MoveSkill");
        BOweaponSkills = Resources.LoadAll<BuyableObject_Skill>($"ScriptableObejct/Skill/1WeaponSkill");
        BOpassiveSkills = Resources.LoadAll<BuyableObject_Skill>($"ScriptableObejct/Skill/2PassiveSkill");
    }
    
    void GenerateSkillUI()
    {
        LoadSkillList();
    
        Transform[] targets = { moveSkillT, weaponSkillT, passiveSkillT };
        BuyableObject_Skill[][] skillGroups = { BOmoveSkills, BOweaponSkills, BOpassiveSkills };

        for (int i = 0; i < 3; i++)
        {
            int j = 0;
            foreach (var bo in skillGroups[i])
            {
                var slot = Instantiate(skillSlot, targets[i]);
                var script = slot.GetComponent<RepairShopSkillSlot>();
                script.Init(bo, this, j++);
                repairShopSkills[i].Add(script);
            }
        }
    }

    public void BuyingSkill()
    {
        if (selectedSkillIndex == -1 || selectedSkillType == -1)
            return;
        var skill = repairShopSkills[selectedSkillType][selectedSkillIndex];
        currentSkills.Add(skill);
        skill.isBought = true;
        Receipt.ReceiptBuySkill(skill.nameText.text, skill.iconImage.sprite);
    }

    public void SkillShopReset(bool refunding)
    {
        if (refunding)
        {
            currentSkills.Clear();
            Receipt.ReceiptRefundSkill();
        }
        
        for (int i = 0; i < repairShopSkills.Length; i++)
        {
            foreach (var slot in repairShopSkills[i])
            {
                if (refunding)
                {
                    if (slot.isBought)
                    {
                        RepairShop.currentMoney += slot.price;
                        slot.isBought = false;
                    }
                    slot.Selected(false); // 환불 중이면 다 선택 해제
                }
                else
                {
                    // 환불이 아닐 경우, 구매된 것만 선택 표시
                    slot.Selected(slot.isBought);
                }
            }
        }
        
        selectedSkillIndex = -1;
        selectedSkillType = -1;
    }
    
    public void ReadSkillInfo(RepairShopSkillSlot ClickedSkill)
    {
        SkillShopReset(false);
        RepairShop.totalPrice = ClickedSkill.price;
        selectedSkillIndex = ClickedSkill.index;
        selectedSkillType = ClickedSkill.skillType;
        ClickedSkill.Selected(true);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
        RepairShop.ErrorMessage.SetActive(false);
        Debug.Log($"{selectedSkillIndex} , {selectedSkillType}");
    }
}
