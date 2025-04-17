using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShopWeapon : MonoBehaviour
{
   [SerializeField] private GameObject weaponSlotPrefab;
   [SerializeField] private List<Weapon> weapons = new List<Weapon>();
   [SerializeField] private List<BuyableObject> buyableWeapons = new List<BuyableObject>();
   
   public void init()
   {
      for (int i = 0; i < buyableWeapons.Count; i++)
      {
         GameObject weaponSlot = Instantiate(weaponSlotPrefab, transform);
         weaponSlot.name = "WeaponSlot " + i;
         
         Weapon weapon = weaponSlot.GetComponent<Weapon>();
         
         weapon.buyableWeapon = buyableWeapons[i];
         weapon.init();
         weapons.Add(weapon);
      }
   }
}
