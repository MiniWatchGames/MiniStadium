using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShopWeapon : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopReceipt Receipt;
    [SerializeField] private GameObject weaponSlotPrefab;
    [SerializeField] private Transform weaponSlotParent;
    private List<BuyableObject_Weapon> BOweaponList;
    private List<RepairShopWeaponSlot> RepairShopWeapons;
    public RepairShopWeaponSlot currentWeapon;
    public RepairShopWeaponSlot selectedWeapon;

    public void init()
    {
        GenerateWeaponUI();
    }
    
    void GenerateWeaponUI()
    {
        RepairShopWeapons = new List<RepairShopWeaponSlot>();
        
        LoadWeaponList();
        int i = 0;
        
        foreach (var bo in BOweaponList)
        {
            var slot = Instantiate(weaponSlotPrefab, weaponSlotParent);
            var script = slot.GetComponent<RepairShopWeaponSlot>();
            script.Init(bo, this, i++);
            RepairShopWeapons.Add(script);
        }
    }
    
    private void LoadWeaponList()
    {
        BOweaponList = new List<BuyableObject_Weapon>();
        BuyableObject_Weapon[] weapons
            = Resources.LoadAll<BuyableObject_Weapon>("ScriptableObejct/Weapon");
        
        BOweaponList.AddRange(weapons);
    }

    public void BuyingWeapon()
    {
        foreach (var slot in RepairShopWeapons)
            slot.Selected(false);
        
        if (selectedWeapon != null)
        {
            currentWeapon = selectedWeapon;
            currentWeapon.Selected(true);
        }
    }
    
    public void WeaponShopReset(bool refunding)
    {
        if (refunding && currentWeapon != null)
        {
            RepairShop.currentMoney += currentWeapon.price;
            currentWeapon = null;
        }
        
        selectedWeapon = null;
        
        foreach (var slot in RepairShopWeapons)
        {
            if(slot == currentWeapon)
                continue;
            slot.Selected(false);
        }
    }

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
                ClickedWeapon.Selected(false);
                selectedWeapon = null;
            }
            else
            {
                selectedWeapon.Selected(false);
                ClickedWeapon.Selected(true);
                selectedWeapon = ClickedWeapon;
                RepairShop.totalPrice += ClickedWeapon.price;
            }
        }
        else // 이전에 선택한 무기가 없거나, 구매된 무기인 경우
        {
            ClickedWeapon.Selected(true);
            selectedWeapon = ClickedWeapon;
            RepairShop.totalPrice += ClickedWeapon.price;
        }
        Receipt.ReceiptUpdateSkill(false);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
        RepairShop.ErrorMessage.SetActive(false);
    }
}
