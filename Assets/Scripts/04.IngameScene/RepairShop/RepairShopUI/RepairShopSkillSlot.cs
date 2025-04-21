using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopSkillSlot : MonoBehaviour
{
    [SerializeField] private RepairShopSkill _manager;
    [SerializeField] private GameObject checkbox;
    
    [SerializeField] public Image iconImage;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] public int skillType;
    [SerializeField] public int weaponType;
    [SerializeField] public int index;
    [SerializeField] public int price;
    [SerializeField] private string description;
    
    public void Init(BuyableObject_Skill BOdata, RepairShopSkill manager)
    {
        _manager = manager;
        iconImage.sprite = BOdata.icon;
        nameText.text = BOdata.name;
        price = BOdata.price;
        priceText.text = $"{BOdata.price.ToString()}g";
        description = BOdata.description;
        weaponType = BOdata.weaponType;   // 0=None 1=Ranged 2=Melee
        skillType = BOdata.skillType;   // 0=MoveSkill 1=WeaponSkill 2=Passive
    }
    
    public void Selected(bool selected)
    {
        checkbox.SetActive(selected);
    }

    public void OnClick()
    {
        
    }
}
