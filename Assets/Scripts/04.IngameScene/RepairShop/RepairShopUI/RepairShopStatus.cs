using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopStatus : MonoBehaviour
{
    private RepairShop _repairShop;
    
    // 스테이터스 최대 업그레이드 수
    private int _maxUpgradeCount = 5;
    // 스테이터스 구분 0=체력 1=방어 2=이동속도
    private int[] _repairShopStatuses = new []{0, 1, 2};
    private List<Products> _productList;

    public void Start()
    {
        _repairShop = this.GetComponent<RepairShop>();
    }

    // 상점 스테이터스 초기화
    public void SetRepairShopStatus(Transform[] targetGroupLayers, GameObject ButtonPrefab, GameObject PricePrefab)
    {
        var textbox = new GameObject[]{new GameObject(), new GameObject(), new GameObject()};
        
        for (int i = 0; i < _repairShopStatuses.Length; i++)
        {
            for (int j = 0; j < _maxUpgradeCount; j++)
            {
                // 버튼 생성
                var currentButton = Instantiate(ButtonPrefab, targetGroupLayers[i]);
                
                // 색 지정
                if (j == 0)
                    SetColor(i, currentButton);
                else
                    SetColor(-1, currentButton);
            }
            // 가격 텍스트UI 생성
            var priceText = Instantiate(PricePrefab, targetGroupLayers[i]);
            textbox[i] = priceText;
        }
        
        // 스테이터스 상품 목록 생성
        _productList = new List<Products>
        {
            new Products("체력", 100, textbox[0]),
            new Products("방어력", 100, textbox[1]),
            new Products("이동속도", 100, textbox[2]),
        };

        // 가격 할당
        foreach (var Products in _productList)
        {
            _repairShop.SetPrice(Products.price, Products.priceText);
        }
    }
    
    // 색 지정
    private void SetColor(int statusType, GameObject statusButton)
    {
        var buttonColor = statusButton.GetComponent<Image>();
        
        Color currentColor = default;
        
        // 스테이터스 타입 구분
        if (statusType == -1)
            currentColor = Color.white;
        else if (statusType == 0)                
            currentColor = Color.red;
        else if (statusType == 1)
            currentColor = Color.green;
        else if (statusType == 2)
            currentColor = Color.blue;
        
        // 적용
        buttonColor.color = currentColor;
    }
    
    public void Init(RepairShop shop)
    {
        this._repairShop = shop;
    }
}
