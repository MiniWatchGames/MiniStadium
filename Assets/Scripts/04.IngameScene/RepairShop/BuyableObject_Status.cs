using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buyable_Status", menuName = "ScriptableObjects/Status", order = 1)]
public class BuyableObject_Status : ScriptableObject
{
    public int type; // 0=HP 1=Armor 2=Speed
    public string statusName;
    public string description;
    
    public Color NormalColor;
    public Color HighlightedColor;
    public Color PressedColor;
    public Color SelectedColor;
    public Color DisabledColor;
}