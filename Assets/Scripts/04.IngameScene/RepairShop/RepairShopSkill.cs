using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RepairShopSkill : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopReceipt Receipt;
    
    // 프리팹 관련
    [SerializeField] private GameObject skillSlot;
    [SerializeField] private GameObject missingWeapon;
    [SerializeField] private Transform moveSkillT;
    [SerializeField] private Transform weaponSkillT;
    [SerializeField] private Transform passiveSkillT;
    
    // 데이터 할당
    private GameObject[][] repairShopSkills;

    public BuyableObject_Skill[] BOmoveSkills;
    public BuyableObject_Skill[] BOweaponSkills;
    public BuyableObject_Skill[] BOpassiveSkills;

    public RepairShopSkillSlot[][] selectedSkills;
    public RepairShopSkillSlot[][] boughtSkills;

    private int[] maxPerType = { 1, 2, 3 }; // 스킬 타입에 따른 최대 구매 량
    public int[] remainingSkillsToBuy = { 1, 2, 3 }; // 각 타입별 남은 선택 가능 횟수
    
    public void init(RepairShop _repairShop)
    {
        GenerateSkillUI(_repairShop);
    }
    
    // 스킬 탭 UI 초기화
    void GenerateSkillUI(RepairShop _repairShop)
    {
        selectedSkills = new RepairShopSkillSlot[3][];
        boughtSkills = new RepairShopSkillSlot[3][];
        repairShopSkills = new GameObject[3][];
        RepairShop = _repairShop;
        for (int i = 0; i < 3; i++)
        {
            selectedSkills[i] = new RepairShopSkillSlot[maxPerType[i]];
            boughtSkills[i] = new RepairShopSkillSlot[maxPerType[i]];
        }

        LoadSkillList();

        Transform[] parents = { moveSkillT, weaponSkillT, passiveSkillT };
        BuyableObject_Skill[][] skillGroups = { BOmoveSkills, BOweaponSkills, BOpassiveSkills };

        for (int i = 0; i < 3; i++)
        {
            repairShopSkills[i] = new GameObject[skillGroups[i].Length];
            for (int j = 0; j < skillGroups[i].Length; j++)
            {
                var slot = Instantiate(skillSlot, parents[i]);
                var script = slot.GetComponent<RepairShopSkillSlot>();
                script.Init(skillGroups[i][j], this, j);
                repairShopSkills[i][j] = slot;

                if (i == 1) slot.SetActive(false);
            }
        }
    }
    
    // 구현된 스킬 정보 로드
    void LoadSkillList()
    {
        BOmoveSkills = Resources.LoadAll<BuyableObject_Skill>
            ($"ScriptableObejct/Skill/0MoveSkill");
        BOweaponSkills = Resources.LoadAll<BuyableObject_Skill>
            ($"ScriptableObejct/Skill/1WeaponSkill");
        BOpassiveSkills = Resources.LoadAll<BuyableObject_Skill>
            ($"ScriptableObejct/Skill/2PassiveSkill");
    }
    
    // 무기 타입에 따른 무기 전용기 탭 구성
    public void SetWeaponSkillUI(int weaponType)
    {
        missingWeapon.SetActive(weaponType == -1);
        foreach (var slot in repairShopSkills[1])
        {
            var script = slot.GetComponent<RepairShopSkillSlot>();
            slot.SetActive(script.weaponType == weaponType);
        }
    }
    
    // 스킬 클릭 시 정보를 읽어들이는
    public void ReadSkillInfo(RepairShopSkillSlot clicked)
    {
        var error = RepairShop.ErrorMessage;
        int type = clicked.skillType;
        
        error.SetActive(false);
        
        // 이미 선택한 슬롯이면 제거
        for (int i = 0; i < maxPerType[type]; i++)
        {
            if (selectedSkills[type][i] == clicked)
            {
                selectedSkills[type][i].Selected(false);
                RepairShop.totalPrice -= clicked.price;
                selectedSkills[type][i] = null;
                remainingSkillsToBuy[type]++; // 선택 해제시 남은 선택 가능 횟수 증가

                for (int j = 0; j < Receipt.receiptSlots[type].Length; j++)
                {
                    if(Receipt.receiptSlots[type][j].ID == clicked.index)
                        Receipt.ResetTargetSlot(Receipt.receiptSlots[type][j], type);
                }
                
                RepairShop.UpdateMoneyText(RepairShop.totalPrice);
                return;
            }
        }
        
        // 최대 선택 가능 개수를 초과하지 않도록 제한
        if (remainingSkillsToBuy[type] <= 0)
        {
            error.GetComponent<TextMeshProUGUI>().text = "더 선택할 수 없습니다.";
            error.SetActive(true);
            return; // 더 이상 선택할 수 없으면 종료
        }

        // 빈 슬롯에 추가
        for (int i = 0; i < maxPerType[type]; i++)
        {
            if (selectedSkills[type][i] == null)
            {
                selectedSkills[type][i] = clicked;
                clicked.Selected(true);
                RepairShop.totalPrice += clicked.price;
                remainingSkillsToBuy[type]--; // 선택 시 남은 선택 가능 횟수 감소
                Receipt.ReceiptUpdateSlot(false, type);
                RepairShop.UpdateMoneyText(RepairShop.totalPrice);
                return;
            }
        }

        // 꽉 차면 가장 먼저 선택된 것 제거 후 교체
        selectedSkills[type][0].Selected(false);
        RepairShop.totalPrice -= selectedSkills[type][0].price;
        remainingSkillsToBuy[type]++; // 선택 해제시 남은 선택 가능 횟수 증가

        for (int i = 1; i < maxPerType[type]; i++)
        {
            selectedSkills[type][i - 1] = selectedSkills[type][i];
        }

        selectedSkills[type][maxPerType[type] - 1] = clicked;
        clicked.Selected(true);
        RepairShop.totalPrice += clicked.price;
        remainingSkillsToBuy[type]--; // 선택 시 남은 선택 가능 횟수 감소

        Receipt.ReceiptUpdateSlot(false, type);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
    }
    
    // 스킬 구매 처리
    public void BuyingSkill()
    {
        for (int i = 0; i < 3; i++) // type
        {
            for (int j = 0; j < maxPerType[i]; j++) // index
            {
                var selected = selectedSkills[i][j];
                if (selected != null)
                {
                    boughtSkills[i][j] = selected;
                    boughtSkills[i][j].isBought = true;
                    boughtSkills[i][j].Selected(true);
                    CheckboxAlpha(true, i, j);
                }
            }
        }
    }

    // 환불 및 리셋
    public void SkillShopReset(bool refunding)
    {
        if (refunding)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < maxPerType[i]; j++)
                {
                    if (boughtSkills[i][j] != null)
                    {
                        var slot = boughtSkills[i][j];
                        var color = slot.checkbox.GetComponent<Image>().color;
                        color.a = 0.5f;
                        slot.checkbox.GetComponent<Image>().color = color;
                        RepairShop.currentMoney += slot.price;
                        slot.isBought = false;
                        slot.Selected(false);
                        boughtSkills[i][j] = null;
                    }
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < maxPerType[i]; j++)
            {
                if (selectedSkills[i][j] != null)
                {
                    selectedSkills[i][j].Selected(refunding ? false : selectedSkills[i][j].isBought);
                    selectedSkills[i][j] = null;
                }
            }
            remainingSkillsToBuy[i] = i + 1;
        }
    }
    
    // 체크 박스의 투명도 제어
    public void CheckboxAlpha(bool buying, int i, int j)
    {
        if (boughtSkills[i][j] == null) return;
        var img = boughtSkills[i][j].checkbox.GetComponent<Image>();
        var color = img.color;
        color.a = buying ? 1f : 0.5f;
        img.color = color;
    }
    
    
}
