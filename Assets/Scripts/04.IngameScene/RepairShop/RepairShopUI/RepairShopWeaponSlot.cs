using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopWeaponSlot : MonoBehaviour
{
    [SerializeField] private RepairShopWeapon _manager;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] public int type;
    [SerializeField] public int index;
    [SerializeField] public int price;
    [SerializeField] private string description;
    //private bool _active = false;
    
    [SerializeField] private GameObject checkbox;
    
    public void Init(BuyableObject_Weapon BOdata, RepairShopWeapon manager, int _index)
    {
        iconImage.sprite = BOdata.icon;
        nameText.text = BOdata.name;
        price = BOdata.price;
        priceText.text = $"{BOdata.price.ToString()}g";
        description = BOdata.description;
        type = BOdata.type;   // 0=Ranged 1=Melee
        index = _index;
        _manager = manager;
    }

    public void Selected(bool selected)
    {
        checkbox.SetActive(selected);
    }

    public void OnClick()
    {
        _manager.ReadWeaponInfo(this);
    }
}
