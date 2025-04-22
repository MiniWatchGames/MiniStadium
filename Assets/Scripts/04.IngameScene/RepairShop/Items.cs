using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemInfo
{
  public string name;
  public Sprite icon;
  public GameObject prefab;
  public RepairShopEnums.WeaponType WeaponType;
  public RepairShopEnums.WeaponRange WeaponRange;
  public float damage;
  public int price;
  public string description;
  public SkillObject skills;
  
}

[CreateAssetMenu(fileName = "Items", menuName = "ScriptableObjects/Items", order = 1)]
public class Items : ScriptableObject
{
   public List<ItemInfo> slotInfos = new List<ItemInfo>();
   
}
