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
    public GameObject ErrorMessage => errorMessage;
    [SerializeField] private GameObject errorMessage;
    
    // 하위 탭
    [SerializeField] private RepairShopStatus RepairShopStatus;
    [SerializeField] private RepairShopWeapon RepairShopWeapon;
    [SerializeField] private RepairShopSkill RepairShopSkill;
    [SerializeField] private RepairShopReceipt RepairShopReceipt;
    
    // 금액
    [SerializeField] private int _startingMoney = 3000;
    [SerializeField] public int currentMoney;
    [SerializeField] public int totalPrice = 0;
    [SerializeField] public TMP_Text currentMoneyText;
    
    // 설명 란
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    
    void Start()
    {
        errorMessage.SetActive(false);
        currentMoney = _startingMoney;
        UpdateMoneyText(0);
        RepairShopStatus.init(this);
        RepairShopSkill.init(this);
        RepairShopWeapon.init(this);
    }

    // 리셋 버튼 클릭 시 환불 절차
    public void OnClickRefundButton()
    {
        if (currentMoney < 200)
        {
            errorMessage.GetComponent<TextMeshProUGUI>().text = "자금이 부족합니다.";
            errorMessage.SetActive(true);
            return;
        }
        currentMoney -= 200;
        RepairShopStatus.StatusReset(true);
        RepairShopWeapon.WeaponShopReset(true);
        RepairShopSkill.SkillShopReset(true);
        RepairShopReceipt.ReceiptRefundAll();
        SetDescription("","");
        ResetPrice();
    }
    
    public void ResetPrice()
    {
        totalPrice = 0;
        UpdateMoneyText(0);
    }
    
    // 가격 및 보유금액 갱신
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

            RepairShopWeapon.BuyingWeapon();
            RepairShopStatus.StatusPurchasing();
            RepairShopSkill.BuyingSkill();
            RepairShopReceipt.ReceiptUpdateSlot(true, 0);
            SetDescription("","");
            
            ResetPrice();
        }
        else // 실패
        {
            errorMessage.GetComponent<TextMeshProUGUI>().text = "자금이 부족합니다.";
            errorMessage.SetActive(true);
        }
    }

    public void SetDescription(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }
}
