using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RepairShop : MonoBehaviour
{
    [SerializeField] private GameObject RepairShopUI;
    [SerializeField] public List<GameObject> RepairShopUpperTabs;
    [SerializeField] public List<GameObject> RepairShopLowerTabs;
    [SerializeField] public GameObject RepairShopStatus;
    [SerializeField] public GameObject RepairShopWeapon;
    [SerializeField] public List<GameObject> RepairShopWeaponList = new List<GameObject>();
    public Sprite EmptyImage;
    public int currentMoney;
    public TMP_Text currentMoneyText;
    
    // Start is called before the first frame update
    void Start()
    {
        currentMoney = 30000;
        currentMoneyText.text ="가격 : " + currentMoney.ToString();
        RepairShopStatus.GetComponent<RepairShopStatus>().init();
        RepairShopWeapon.GetComponent<RepairShopWeapon>().init();
    }

    string SetMoneyText(int Money)
    {
        //string format = $"가격 : {currentMoney} - {Money} = {currentMoney - Money}";
        string format = $"가격 : {Money}/{currentMoney}";
        if (Money <= currentMoney)
        {
            return format;
        }
        else
        {
            return "가격이 현재 가진 금액보다 많습니다.";
        }
        
    }

    public void RemoveImage(Image image)
    {
        image.sprite = EmptyImage;
    }
    
    
}
