using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buyable", menuName = "ScriptableObjects/Buyable", order = 1)]
public class BuyableObject : ScriptableObject
{
   public Sprite icon;
   
   public string name;
   
   public int price;
   
   public string description;
   
   
}
