using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopWeapon : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopSkill RepairShopSkill;
    [SerializeField] private RepairShopReceipt Receipt;
    [SerializeField] private GameObject weaponSlotPrefab;
    [SerializeField] private Transform weaponSlotParent;
    private List<BuyableObject_Weapon> BOweaponList;
    private List<RepairShopWeaponSlot> RepairShopWeapons;
    public RepairShopWeaponSlot currentWeapon;
    public RepairShopWeaponSlot selectedWeapon;
    public RepairShopWeaponSlot baseWeapon;

    public void init(RepairShop _repairShop)
    {
        GenerateWeaponUI(_repairShop);
    }
    
    // 무기 탭 UI 초기화
    void GenerateWeaponUI(RepairShop _repairShop)
    {
        RepairShopWeapons = new List<RepairShopWeaponSlot>();
        RepairShop = _repairShop;
        LoadWeaponList();
        int i = 0;
        
        foreach (var bo in BOweaponList)
        {
            var slot = Instantiate(weaponSlotPrefab, weaponSlotParent);
            var script = slot.GetComponent<RepairShopWeaponSlot>();
            script.Init(bo, this, i++ + 1);
            RepairShopWeapons.Add(script);
        }

        // 기본 무기 설정
        baseWeapon = RepairShopWeapons[1];
        
        // 초기화 (null 참조 방지)
        Receipt.ReceiptRefundAll();
    }
    
    // 구현된 무기 정보 로드
    private void LoadWeaponList()
    {
        BOweaponList = new List<BuyableObject_Weapon>();
        BuyableObject_Weapon[] weapons
            = Resources.LoadAll<BuyableObject_Weapon>("ScriptableObejct/Weapons");
        
        BOweaponList.AddRange(weapons);
    }

    // 무기 구매 처리
    public void BuyingWeapon()
    {
        if (selectedWeapon != null)
        {
            if (currentWeapon != null)
            {
                CheckboxAlpha(false);
            }
            currentWeapon = selectedWeapon;
            currentWeapon.Selected(true);
            CheckboxAlpha(true);
            RepairShopSkill.SetWeaponSkillUI(currentWeapon.type);
        }
        foreach (var slot in RepairShopWeapons)
        {
            if (slot == currentWeapon) continue;
            
            slot.Selected(false);
            
            if(currentWeapon != null)
                slot.iconImage.GetComponent<Button>().interactable = false;
        }
    }

    // 체크 박스의 투명도 제어
    public void CheckboxAlpha(bool buying)
    {
        var color = currentWeapon.checkbox.GetComponent<Image>().color;
        if (buying)
            color.a = 1f;
        else
            color.a = 0.5f;
        currentWeapon.checkbox.GetComponent<Image>().color = color;
    }
    
    // 환불 및 리셋
    public void WeaponShopReset(bool refunding)
    {
        if (refunding && currentWeapon != null)
        {
            RepairShop.currentMoney += currentWeapon.price;
            
            CheckboxAlpha(false);
            currentWeapon = null;
           
        }
        
        selectedWeapon = null;
        
        foreach (var slot in RepairShopWeapons)
        {
            if(refunding)
                slot.iconImage.GetComponent<Button>().interactable = true;
            if(slot == currentWeapon)
                continue;
            slot.Selected(false);
        }
 		if (currentWeapon == null)
            RepairShopSkill.SetWeaponSkillUI(-1);
    }
    
    // 무기 클릭 시 정보를 읽어들이는
    public void ReadWeaponInfo(RepairShopWeaponSlot ClickedWeapon)
    {
        if (ClickedWeapon == currentWeapon)
            return;
        
        // 이전에 선택 한 미구매 무기가 있을 시
        if (selectedWeapon != null && selectedWeapon != currentWeapon)
        {
            RepairShop.totalPrice -= selectedWeapon.price;

            if (selectedWeapon == ClickedWeapon)
            {
            	RepairShop.SetDescription("", "");
                Receipt.ResetTargetSlot(Receipt.receiptSlots[3][0],3);
                ClickedWeapon.Selected(false);
                selectedWeapon = null;
   				RepairShopSkill.SetWeaponSkillUI(-1);
                Receipt.ReceiptUndo(Receipt.receiptSlots[1][0], 0);
                Receipt.ReceiptUndo(Receipt.receiptSlots[1][1], 1);
            }
            else
            {
 				RepairShop.SetDescription(ClickedWeapon.nameText.text, ClickedWeapon.description);
                selectedWeapon.Selected(false);
                ClickedWeapon.Selected(true);
                selectedWeapon = ClickedWeapon;
                RepairShop.totalPrice += ClickedWeapon.price;
   			 	RepairShopSkill.SetWeaponSkillUI(selectedWeapon.type);
                Receipt.ReceiptUndo(Receipt.receiptSlots[1][0], 0);
                Receipt.ReceiptUndo(Receipt.receiptSlots[1][1], 1);
            }
        }
        else // 이전에 선택한 무기가 없거나, 구매된 무기인 경우
        {
 			RepairShop.SetDescription(ClickedWeapon.nameText.text, ClickedWeapon.description);		
            ClickedWeapon.Selected(true);
            selectedWeapon = ClickedWeapon;
            RepairShop.totalPrice += ClickedWeapon.price;
  			RepairShopSkill.SetWeaponSkillUI(selectedWeapon.type);
        }
        Receipt.ReceiptUpdateSlot(false, 3);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
        RepairShop.ErrorMessage.SetActive(false);
    }
}
