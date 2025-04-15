using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShop : MonoBehaviour
{
    private RepairShopStatus _repairShopStatus;
    
    // 골드 관련
    private int _price = 0;
    [SerializeField] private int _startingGold = 600;
    public int currentGold => _currentGold;
    [SerializeField] private int _currentGold = 0;
    [SerializeField] private TextMeshProUGUI _priceText;
    
    // 스테이터스 관련

    private void Awake()
    {
        
    }

    private void Start()
    {
        // 골드 초기화
        _currentGold = _startingGold;
        
        // 가격 정보 초기화
        UpdateCurrentPrice();
    }


    #region 구매 관련
    
    // 구매
    private bool PurchaseItem()
    {
        if (_currentGold >= _price)
        {
            _currentGold -= _price;
            _price = 0;
            UpdateCurrentPrice();
            return true;
        }
        else
        {
            Debug.Log("구매 실패");
            return false;
        }
    }
    
    // 가격 UI 갱신
    private void UpdateCurrentPrice()
    {
        _priceText.text = "가격 : " + _price.ToString("N0") + "/" 
                          + _currentGold.ToString("N0") + "g";
    }

    #endregion
}
