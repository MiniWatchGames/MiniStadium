//1
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopWeaponSlot : MonoBehaviour
{
    [SerializeField] private RepairShopWeapon _manager;
    [SerializeField] public GameObject checkbox;
    
    [SerializeField] public Image iconImage;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] public int type;
    [SerializeField] public int index;
    [SerializeField] public int price;
    [SerializeField] public string description;
    
    public void Init(BuyableObject_Weapon BOdata, RepairShopWeapon manager, int _index)
    {
        _manager = manager;
        iconImage.sprite = BOdata.icon;
        nameText.text = BOdata._name;
        price = BOdata.price;
        priceText.text = $"{BOdata.price.ToString()}g";
        description = BOdata.description;
        type = BOdata.type;   // 0=None 1=Ranged 2=Melee
        index = _index;
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
