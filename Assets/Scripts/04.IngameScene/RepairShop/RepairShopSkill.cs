using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RepairShopSkill : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopReceipt Receipt;
    
    // 프리팹 관련
    [SerializeField] private GameObject skillSlot;
    [SerializeField] private Transform moveSkillT;
    [SerializeField] private Transform weaponSkillT;
    [SerializeField] private Transform passiveSkillT;
    
    // 데이터 할당
    private RepairShopSkillSlot[][] repairShopSkills;
    public BuyableObject_Skill[] BOmoveSkills;
    public BuyableObject_Skill[] BOweaponSkills;
    public BuyableObject_Skill[] BOpassiveSkills;
    
    public RepairShopSkillSlot[] selectedSkills;
    public RepairShopSkillSlot[] boughtSkills;
    
    public void init()
    {
        GenerateSkillUI();
    }
    
    void GenerateSkillUI()
    {
        selectedSkills = new RepairShopSkillSlot[3];
        boughtSkills = new RepairShopSkillSlot[3];

        LoadSkillList();

        Transform[] targets = { moveSkillT, weaponSkillT, passiveSkillT };
        BuyableObject_Skill[][] skillGroups = { BOmoveSkills, BOweaponSkills, BOpassiveSkills };

        repairShopSkills = new RepairShopSkillSlot[3][];

        for (int i = 0; i < 3; i++)
        {
            repairShopSkills[i] = new RepairShopSkillSlot[skillGroups[i].Length];

            for (int j = 0; j < skillGroups[i].Length; j++)
            {
                var slot = Instantiate(skillSlot, targets[i]);
                var script = slot.GetComponent<RepairShopSkillSlot>();
                script.Init(skillGroups[i][j], this, j);
                repairShopSkills[i][j] = script;
            }
        }
    }
    
    void LoadSkillList()
    {
        BOmoveSkills = Resources.LoadAll<BuyableObject_Skill>
            ($"ScriptableObejct/Skill/0MoveSkill");
        BOweaponSkills = Resources.LoadAll<BuyableObject_Skill>
            ($"ScriptableObejct/Skill/1WeaponSkill");
        BOpassiveSkills = Resources.LoadAll<BuyableObject_Skill>
            ($"ScriptableObejct/Skill/2PassiveSkill");
    }

    public void BuyingSkill()
    {
        if (selectedSkills == null)
            return;

        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if (selectedSkills[i] != null)
            {
                if (boughtSkills[i] != null)
                {
                    boughtSkills[i].isBought = false;
                    boughtSkills[i].Selected(false);
                }
                boughtSkills[i] = selectedSkills[i];
                boughtSkills[i].isBought = true;
            }
        }
    }

    public void SkillShopReset(bool refunding)
    {
        if (refunding)
        {
            boughtSkills = new RepairShopSkillSlot[3];
            selectedSkills = new RepairShopSkillSlot[3];
        }
        
        foreach (var type in repairShopSkills)
        {
            foreach (var slot in type)
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
    }
    
    // 스킬 클릭 시
    public void ReadSkillInfo(RepairShopSkillSlot ClickedSkill)
    {
        int type = ClickedSkill.skillType;
        var prevSkill = selectedSkills[type];

        // 이전에 선택한 같은 타입의 미구매 스킬이 있을 시
        if (prevSkill != null && !prevSkill.isBought)
        {
            RepairShop.totalPrice -= prevSkill.price;

            if (prevSkill == ClickedSkill)
            {
                ClickedSkill.Selected(false);
                selectedSkills[type] = null;
            }
            else
            {
                prevSkill.Selected(false);
                ClickedSkill.Selected(true);
                selectedSkills[type] = ClickedSkill;
                RepairShop.totalPrice += ClickedSkill.price;
            }
        }
        else // 이전에 선택한 같은 타입의 스킬이 없거나, 구매된 스킬인 경우
        {
            ClickedSkill.Selected(true);
            selectedSkills[type] = ClickedSkill;
            RepairShop.totalPrice += ClickedSkill.price;
        }
        Receipt.ReceiptUpdateSkill(false);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
        RepairShop.ErrorMessage.SetActive(false);
    }
}
