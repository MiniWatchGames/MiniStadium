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
    [SerializeField] private GameObject _statusButtonPrefab;
    [SerializeField] private Transform[] _targetGroupLayer;
    [SerializeField] private GameObject _statusPriceTextPrefab;

    private void Awake()
    {
        _repairShopStatus = gameObject.AddComponent<RepairShopStatus>();
    }

    private void Start()
    {
        // 골드 초기화
        _currentGold = _startingGold;
        
        // 스테이터스 클래스 할당
        _repairShopStatus.Init(this);
        
        // 스테이터스 메뉴 항목 초기화
        _repairShopStatus.SetRepairShopStatus(_targetGroupLayer
            , _statusButtonPrefab, _statusPriceTextPrefab);
        
        // 가격 UI 초기화
        UpdateCurrentPrice();
    }


    #region 구매 관련
    
    private void PurchaseItem(Products data)
    {
        
    }

    // 가격 UI 갱신
    private void UpdateCurrentPrice()
    {
        _priceText.text = "가격 : " + _price.ToString("N0") + "/" 
                          + _currentGold.ToString("N0") + "g";
    }

    // 가격 지정
    public void SetPrice(int price, GameObject textUI)
    {
        textUI.GetComponent<TextMeshProUGUI>().text = price.ToString("N0") + "g";
    }

    #endregion
}
