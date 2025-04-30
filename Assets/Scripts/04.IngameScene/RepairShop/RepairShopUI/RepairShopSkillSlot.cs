using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopSkillSlot : MonoBehaviour
{
    [SerializeField] private RepairShopSkill _manager;
    
    [SerializeField] public Image iconImage;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] public int skillType;
    [SerializeField] public int weaponType;
    [SerializeField] public int index;
    [SerializeField] public int price;
    [SerializeField] private string description;
    
    [SerializeField] public GameObject checkbox;
    [SerializeField] public bool isBought = false;
    
    public void Init(BuyableObject_Skill BOdata, RepairShopSkill manager, int _index)
    {
        _manager = manager;
        iconImage.sprite = BOdata.icon;
        nameText.text = BOdata.name;
        price = BOdata.price;
        priceText.text = $"{BOdata.price.ToString()}g";
        description = BOdata.description;
        weaponType = BOdata.weaponType;   // 0=None 1=Ranged 2=Melee
        skillType = BOdata.skillType;   // 0=MoveSkill 1=WeaponSkill 2=Passive
        index = _index;
    }
    
    public void Selected(bool selected)
    {
        checkbox.SetActive(selected);
    }

    public void OnClick()
    {
        if (isBought)
            return;
        _manager.ReadSkillInfo(this);
    }
}
