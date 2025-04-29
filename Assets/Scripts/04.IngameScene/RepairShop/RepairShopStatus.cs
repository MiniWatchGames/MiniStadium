using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopStatus : MonoBehaviour
{
    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopReceipt Receipt;
    [SerializeField] private BuyableObject_Status[] types;
    
    // 스테이터스 최대 업그레이드 가능한 수 + 기본 블럭 1
    private int _maxUpgradeCount = 5;
    
    // 스테이터스 가격
    private int _statusOriginalPrice = 100;
    private int _statusAddtivePrice = 100;
    private int _totalWorth;
    private List<GameObject> _priceTags;
    
    // 스테이터스 버튼
    [SerializeField] private GameObject statusPrefab;
    [SerializeField] private GameObject statusGroupPrefab;
    [SerializeField] private Transform statusGroupParent;
    [SerializeField] private Transform statusPreviewParent;
    [SerializeField] private GameObject statusNamePrefab;
    [SerializeField] private GameObject pricePrefab;
    private RepairShopStatusButton[,] _buttonsInRows;
    
    // 마지막으로 선택한 버튼의 인덱스 값
    private List<int> _lastButtonIndexes;
    
    public void init()
    {
        SetRepairShopStatus();
    }

    // 상점 스테이터스 초기화 및 생성
    public void SetRepairShopStatus()
    {
        LoadStatusList();
        
        int row = types.Length;
        int col = _maxUpgradeCount;
        
        _buttonsInRows = new RepairShopStatusButton[row, col];
        Receipt.PreviewsInRows = new Image[row, col];
        _priceTags = new List<GameObject>(row);
        _lastButtonIndexes = new List<int>(row);
        
        for (int i = 0; i < row; i++)
        {
            // 마지막 버튼 정보 초기화
            _lastButtonIndexes.Add(-1);
            
            // 이름표, 그룹 레이어, receipt 출력 UI 생성
            var group = Instantiate(statusGroupPrefab, statusGroupParent);
            var previewGroup = Instantiate(Receipt.statusGroup, statusPreviewParent);
            var nameTag = Instantiate(statusNamePrefab, group.transform);
            nameTag.GetComponent<TMP_Text>().text = types[i].statusName;
            
            // 버튼 생성, 초기화
            for (int j = 0; j < col; j++)
            {
                var newButton = Instantiate(statusPrefab, group.transform);
                var newButtonData = newButton.GetComponent<RepairShopStatusButton>();
                newButtonData.Init(j, this, _statusOriginalPrice, _statusAddtivePrice, types[i]);
                _buttonsInRows[i, j] = newButtonData;
                
                // 미리보기 생성
                var preview = Instantiate(Receipt.status, previewGroup.transform);
                Receipt.PreviewsInRows[i, j] = preview.GetComponent<Image>();

                if (j == 0)
                {
                    newButtonData.button.interactable = false;
                    Receipt.PreviewsInRows[i, j].color = types[i].DisabledColor;
                }
            }
            // 가격 텍스트UI 생성
            _priceTags.Add(Instantiate(pricePrefab, group.transform));
            StatusSetPrice(0, _priceTags[i]);
        }
    }

    // 스킬 정보 로드
    void LoadStatusList()
    {
        types = Resources.LoadAll<BuyableObject_Status>
            ($"ScriptableObejct/Status");
    }
    
    // 버튼 선택
    public void SelectSameTypeToLeft(RepairShopStatusButton clickedButton)
    {
        RepairShop.ErrorMessage.SetActive(false);

        int row = clickedButton.StatusType;
        int col = clickedButton.StatusIndex;
        int lastIndex = _lastButtonIndexes[row];

        // 금액 반환 및 클릭된 row 초기화
        int value = CalculateRowTotal(row);
        RepairShop.totalPrice -= value;
        
        for (int i = 0; i < _maxUpgradeCount; i++)
            _buttonsInRows[row, i].SetSelected(false);

        // 같은 버튼 다시 클릭 시 위치 정보 삭제
        if (col == lastIndex)
        {
            _lastButtonIndexes[row] = -1;
            RepairShop.SetDescription("", "");
            HighlightSameTypeToLeft(_buttonsInRows[row, col], true);
        }
        else
        {
            for (int i = 1; i <= col; i++)
                _buttonsInRows[row, i].SetSelected(true);
            _lastButtonIndexes[row] = col;
            RepairShop.SetDescription(types[row].statusName, clickedButton.description);
        }

        // 가격 취합
        value = CalculateRowTotal(row);
        RepairShop.totalPrice += value;
    
        // 가격 정보 갱신
        StatusSetPrice(value, _priceTags[row]);
        RepairShop.UpdateMoneyText(RepairShop.totalPrice);
    }
        
    // 리셋
    public void StatusReset(bool refunding)
    {
        if (refunding) // 환불 버튼을 눌렀을 때
        {
            RepairShop.currentMoney += _totalWorth;
            _totalWorth = 0;
        }
        
        for(int i = 0; i < types.Length; i++)
        {
            for (int j = 1; j < _maxUpgradeCount; j++)
            {
                var button = _buttonsInRows[i, j];
                if (refunding)
                {
                    button.isBought = false;
                    button.button.interactable = true;
                    Receipt.ChangeStatusColor(i, j, Color.white);
                }
                button.SetSelected(false);
            }
            _lastButtonIndexes[i] = -1;  // 위치 정보 삭제
            StatusSetPrice(0, _priceTags[i]); // 가격표 갱신
        }
    }

    // 구매
    public void StatusPurchasing()
    {
        for (int i = 0; i < types.Length; i++)
        {
            for (int j = 0; j < _maxUpgradeCount; j++)
            {
                var button = _buttonsInRows[i, j];
                if (!button.isBought && button.isSelected)
                {
                    switch (i)
                    {
                        case 0: Receipt.Count_HP++; break;
                        case 1: Receipt.Count_AR++; break;
                        case 2: Receipt.Count_MV++; break;
                        case 3: Receipt.Count_JP++; break;
                    }

                    _totalWorth += button.StatusPrice;
                    
                    // 구매 된 버튼 비활성화
                    button.isBought = true;
                    button.GetComponent<Button>().interactable = false;
                    
                    // 미리보기 색상 적용
                    var color = button.button.colors.disabledColor;
                    Receipt.ChangeStatusColor(i, j, color);
                }
            }
            StatusSetPrice(0, _priceTags[i]);
        }
    }
    
    // 가격표 갱신
    private void StatusSetPrice(int price, GameObject textUI)
    {
        textUI.GetComponent<TMP_Text>().text = price.ToString("N0") + "g";
    }
    
    // Row 내 가격 취합
    private int CalculateRowTotal(int row)
    {
        int total = 0;

        for (int i = 0; i < _maxUpgradeCount; i++)
        {
            var button = _buttonsInRows[row, i];
            if (!button.isBought && button.isSelected)
                total += button.StatusPrice;
        }
        return total;
    }
    
    // 마우스 오버 하이라이트
    public void HighlightSameTypeToLeft(RepairShopStatusButton hoveredButton, bool highlight)
    {
        int type = hoveredButton.StatusType;
        int index = hoveredButton.StatusIndex;
        var white = Color.white;

        for (int i = 0; i <= index; i++)
        {
            if (!_buttonsInRows[type, i].isSelected)  // 선택된 건 변경 X
            {
                var cb = _buttonsInRows[type, i].button.colors;
                cb.normalColor = highlight ? cb.highlightedColor : white;
                _buttonsInRows[type, i].button.colors = cb;
            }
        }
    }
}
