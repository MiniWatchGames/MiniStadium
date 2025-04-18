using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopStatus : MonoBehaviour
{
    [SerializeField] private RepairShop _repairShop;
    
    // 스테이터스 최대 업그레이드 수
    private int _maxUpgradeCount = 4;
    
    // 스테이터스 가격
    private List<GameObject> _priceTag;
    private int _statusOriginalPrice = 100;
    private int _statusAddtivePrice = 100;
    private int _statusTotalPrice = 0;
    
    // 스테이터스 버튼
    [SerializeField] private GameObject[] _repairShopStatuses;
    [SerializeField] private GameObject _pricePrefab;
    [SerializeField] private Transform[] _targetGroupLayers;
    [SerializeField] private Transform[] _targetGLPreview;
    [SerializeField] private GameObject _priviewPrefab;
    private RepairShopStatusButton[,] _buttonsInRows;
    private Image[,] _previewsInRows;
    
    // 마지막으로 선택한 버튼의 인덱스 값
    private List<int> _lastButtonIndexes;
    
    public void init()
    {
        SetRepairShopStatus();
    }

    // 탭을 닫았을 때 리셋
    public void StatusReset()
    { 
        for(int j = 0; j < _targetGroupLayers.Length; j++)
        {
            for (int i = 0; i < _maxUpgradeCount; i++)
                _buttonsInRows[j, i].SetSelected(false);
            
            _lastButtonIndexes[j] = -1;  // 위치 정보 삭제
            StatusSetPrice(0, _priceTag[j]); // 가격표 갱신
        }
        _statusTotalPrice = 0;
    }

    // 상점 스테이터스 초기화 및 생성
    public void SetRepairShopStatus()
    {
        int row = Mathf.Min(_repairShopStatuses.Length, _targetGroupLayers.Length);
        int col = _maxUpgradeCount;
        
        _buttonsInRows = new RepairShopStatusButton[row, col];
        _previewsInRows = new Image[row, col];
        _priceTag = new List<GameObject>(row);
        _lastButtonIndexes = new List<int>(row);
        
        for (int i = 0; i < row; i++)
        {
            // 마지막 버튼 정보 초기화
            _lastButtonIndexes.Add(-1);
            
            // 버튼 생성, 초기화
            for (int j = 0; j < col; j++)
            {
                var newButton = Instantiate(_repairShopStatuses[i], _targetGroupLayers[i]);
                var newButtonData = newButton.GetComponent<RepairShopStatusButton>();
                newButtonData.Init(i,j, this, _statusOriginalPrice, _statusAddtivePrice);
                _buttonsInRows[i, j] = newButtonData;
                
                // 미리보기 생성
                var preview = Instantiate(_priviewPrefab, _targetGLPreview[i]);
                _previewsInRows[i, j] = preview.GetComponent<Image>();
            }
            // 가격 텍스트UI 생성
            _priceTag.Add(Instantiate(_pricePrefab, _targetGroupLayers[i]));
            StatusSetPrice(0, _priceTag[i]);
        }
    }
    
    // 버튼 선택
    public void SelectSameTypeToLeft(RepairShopStatusButton clickedButton)
    {
        _repairShop.ErrorMessage.SetActive(false);
        
        int row = clickedButton.StatusType;
        int col = clickedButton.StatusIndex;
        int lastIndex = _lastButtonIndexes[row];
        
        // 초기화
        _statusTotalPrice -= CalculateRowTotal(row);
        for (int i = 0; i < _maxUpgradeCount; i++)
            _buttonsInRows[row, i].SetSelected(false);
        
        // 같은 버튼 다시 클릭 시 위치 정보 삭제
        if (col == lastIndex)
            _lastButtonIndexes[row] = -1;
        else
        {
            // 재선택
            for (int i = 0; i <= col; i++)
                _buttonsInRows[row, i].SetSelected(true);
            _lastButtonIndexes[row] = col; // 위치 정보 저장
        }
        // 가격 갱신
        _statusTotalPrice += CalculateRowTotal(row);
        StatusSetPrice(CalculateRowTotal(row), _priceTag[row]);
        _repairShop.totalPrice = _statusTotalPrice;
        _repairShop.UpdateMoneyText(_statusTotalPrice);
    }
    
    // 구매
    public void StatusPurchasing()
    {
        for (int i = 0; i < _repairShopStatuses.Length; i++)
        {
            for (int j = 0; j < _maxUpgradeCount; j++)
            {
                var button = _buttonsInRows[i, j];
                if (!button.isBought && button.isSelected)
                {
                    // 구매 된 버튼 비활성화
                    button.isBought = true;
                    button.GetComponent<Button>().interactable = false;
                    
                    // 미리보기 색상 적용
                    var color = button.GetComponent<Button>().colors.disabledColor;
                    var preview = _previewsInRows[i, j];
                    preview.color = color;
                }
            }
            StatusSetPrice(0, _priceTag[i]);
        }
        // 가격 초기화
        _statusTotalPrice = 0;
    }
    
    // 가격표 갱신
    private void StatusSetPrice(int price, GameObject textUI)
    {
        textUI.GetComponent<TextMeshProUGUI>().text = price.ToString("N0") + "g";
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

    #region 미사용

    /*
     
     */

    #endregion
}
