using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
public class RepairShop : MonoBehaviour
{
    [SerializeField] private GameObject RepairShopUI;
    [SerializeField] private UpperTabs upperTabs;
    [SerializeField] public List<GameObject> RepairShopUpperTabs;
    [SerializeField] public List<GameObject> RepairShopLowerTabs;
    [SerializeField] private RepairShopStatus RepairShopStatus;
    [SerializeField] private RepairShopWeapon RepairShopWeapon;
    [SerializeField] private RepairShopSkill RepairShopSkill;
    public GameObject ErrorMessage => errorMessage;
    [SerializeField] private GameObject errorMessage;
    
    private int _startingMoney = 3000;
    public int currentMoney;
    public int totalPrice = 0;
    public TMP_Text currentMoneyText;
    
    void Start()
    {
        errorMessage.SetActive(false);
        currentMoney = _startingMoney;
        UpdateMoneyText(0);
        RepairShopStatus.init();
    }

    public void OnClickRefundButton()
    {
        if (currentMoney < 200)
        {
            errorMessage.SetActive(true);
            return;
        }
        currentMoney -= 200;
        RepairShopStatus.StatusReset(true);
        RepairShopWeapon.WeaponShopReset(true);
        RepairShopSkill.SkillShopReset(true);
        UpdateMoneyText(0);
    }
    
    public void ResetPrice()
    {
        totalPrice = 0;
        UpdateMoneyText(0);
    }
    
    public void UpdateMoneyText(int price)
    {
        string format = $"가격 : {price}g / {currentMoney}g";
        currentMoneyText.text = format;
    }
    
    // 구매
    public void PurchaseItem()
    {
        // 성공
        if (currentMoney >= totalPrice)
        {
            errorMessage.SetActive(false);
            currentMoney -= totalPrice;
            totalPrice = 0;
            UpdateMoneyText(0);

            // 무기 탭이 열린 경우
            if (RepairShopUpperTabs[0].activeSelf)
                RepairShopWeapon.BuyingWeapon();
            // 스테이터스 탭이 열린 경우
            if (RepairShopUpperTabs[1].activeSelf)
                RepairShopStatus.StatusPurchasing();
            // 스킬 탭이 열린 경우
            if (RepairShopUpperTabs[2].activeSelf)
                RepairShopSkill.BuyingSkill();
        }
        // 실패
        else
        {
            errorMessage.SetActive(true);
        }
    }
}
