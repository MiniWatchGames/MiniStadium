using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairShopWeapon : MonoBehaviour
{
   [SerializeField] private GameObject weaponSlotPrefab;
   [SerializeField] private List<Weapon> weapons = new List<Weapon>();
   [SerializeField] private List<Items> buyableWeapons = new List<Items>();

   delegate void OnWeaponDelegate();
   OnWeaponDelegate onWeaponClickedDelegate;
   Action onWeaponClickedAction;
   //TODO: 아이콘 클릭식 예약된 함수 실행.
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
         //onWeaponClickedAction = weaponClicked;
      }
   }
}
