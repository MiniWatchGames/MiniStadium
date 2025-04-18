using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShopWeapon : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private List<BuyableObject_Weapon> BOweaponList;
    [SerializeField] private List<RepairShopWeaponSlot> RepairShopWeapons;
    [SerializeField] private GameObject weaponSlotPrefab;
    [SerializeField] private Transform weaponSlotParent;
    public int weaponIndex;
    public RepairShopWeaponSlot currentWeapon;

    void Start()
    {
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
            script.Init(bo, this, i);
            RepairShopWeapons.Add(script);
            i++;
        }
    }
    
    private void LoadWeaponList()
    {
        BOweaponList = new List<BuyableObject_Weapon>();
        BuyableObject_Weapon[] weapons
            = Resources.LoadAll<BuyableObject_Weapon>("ScriptableObejct/Weapon");
        
        BOweaponList.Clear();
        BOweaponList.AddRange(weapons);
    }

    public void BuyingWeapon()
    {
        if (currentWeapon == null)
            currentWeapon = RepairShopWeapons[weaponIndex];
    }
    
    public void WeaponShopReset(bool refunding)
    {
        if (refunding)
            currentWeapon = null;
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
