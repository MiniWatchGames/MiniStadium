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
    [SerializeField] public RepairShopStatus RepairShopStatus;
    public GameObject ErrorMessage => errorMessage;
    [SerializeField] private GameObject errorMessage;
    
    private int startingMoney = 30000;
    private int currentMoney;
    public int totalPrice = 0;
    public TMP_Text currentMoneyText;
    
    void Start()
    {
        errorMessage.SetActive(false);
        currentMoney = startingMoney;
        UpdateMoneyText(0);
        RepairShopStatus.init();
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

            // 스테이터스 창이 열린 경우
            if (RepairShopUpperTabs[1].activeSelf)
                RepairShopStatus.StatusPurchasing();
        }
        // 실패
        else
        {
            errorMessage.SetActive(true);
        }
    }
}
