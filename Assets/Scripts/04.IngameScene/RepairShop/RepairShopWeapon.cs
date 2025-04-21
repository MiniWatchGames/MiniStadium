using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShopWeapon : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopReceipt Receipt;
    private List<BuyableObject_Weapon> BOweaponList;
    private List<RepairShopWeaponSlot> RepairShopWeapons;
    [SerializeField] private GameObject weaponSlotPrefab;
    [SerializeField] private Transform weaponSlotParent;
    public int weaponIndex;
    public RepairShopWeaponSlot currentWeapon;

    void Start()
    {
        weaponIndex = -1;
        RepairShopWeapons = new List<RepairShopWeaponSlot>();
        GenerateWeaponUI();
    }

    void GenerateWeaponUI()
    {
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
        // 인덱스가 유효한지 먼저 확인
        if (weaponIndex < 0 || weaponIndex >= RepairShopWeapons.Count)
            return;
        
        if (currentWeapon == null)
        {
            currentWeapon = RepairShopWeapons[weaponIndex];
            Receipt.ReceiptBuyWeapon();
        }
    }
    
    public void WeaponShopReset(bool refunding)
    {
        if (refunding)
        {
            currentWeapon = null;
            Receipt.ReceiptRefundWeapon();
        }
        if (currentWeapon != null) return;
        
        foreach (var slot in RepairShopWeapons)
            slot.Selected(false);
        weaponIndex = -1;
    }

    public void ReadWeaponInfo(RepairShopWeaponSlot ClickedWeapon)
    {
        if (currentWeapon != null)
            return;
        RepairShop.totalPrice = ClickedWeapon.price;
        weaponIndex = ClickedWeapon.index;
        foreach (var slot in RepairShopWeapons)
            slot.Selected(false);
        ClickedWeapon.Selected(true);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
    }
}
