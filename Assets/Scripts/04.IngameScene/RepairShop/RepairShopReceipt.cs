using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopReceipt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private RepairShopWeapon Weapon;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Sprite noneSprite;
    [SerializeField] public GameObject status;
    [SerializeField] public GameObject statusGroup;
    public Image[,] previewsInRows;

    public void ChangeStatusColor(int i, int j, Color color)
    {
        previewsInRows[i, j].color = color;
    }
    
    public void ReceiptRefund()
    {
        weaponImage.sprite = noneSprite;
        weaponText.text = "무기 없음";
    }
    
    public void ReceiptBuyWeapon()
    {
        weaponText.text = Weapon.currentWeapon.nameText.text;
        weaponImage.sprite = Weapon.currentWeapon.iconImage.sprite;
    }
}
