using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buyable_Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class BuyableObject_Weapon : ScriptableObject
{
   public Sprite icon;
   
   public string name;
   
   public int price;
   
   public string description;

   // 0=Ranged 1=Melee
   public int type;
}
