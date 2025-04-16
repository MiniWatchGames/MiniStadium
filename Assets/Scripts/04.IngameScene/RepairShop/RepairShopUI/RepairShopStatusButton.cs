using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepairShopStatusButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 타입 0= 체력, 1= 방어력, 2= 이동속도
    public int StatusType => _statusType;
    private int _statusType;
    // 버튼 순서
    public int StatusIndex => _statusIndex;
    private int _statusIndex;
    // 가격
    public int StatusPrice => _price;
    private int _price;

    private RepairShopStatus _manager;
    
    // 버튼 상태 관련
    public bool isSelected = false;
    public bool isBought = false;
    public Button button;

    // 정보 설정
    public void Init(int statusType, int index, RepairShopStatus manager
        , int originalPrice, int addtivePrice)
    {
        this._statusType = statusType;
        this._statusIndex = index;
        // 업그레이드 레벨 당 가격 (addtivePrice)g증가
        this._price = originalPrice + index * addtivePrice;
        this._manager = manager;
        
        button = GetComponent<Button>();
    }

    public void OnClick()
    {
        _manager.ErrorMessage.SetActive(false);
        // 좌측의 모든 버튼들을 선택
        _manager.SelectSameTypeToLeft(this);
        // 유니티 기본 선택 기능 해제
        EventSystem.current.SetSelectedGameObject(null);
    }

    // 선택 시 비주얼 처리 및 가격정보 전달
    public int SetSelected(bool selected)
    {
        if (isSelected == selected) return 0;
        
        isSelected = selected;
        UpdateVisualState();
        return selected ? _price : -_price;
    }
    
    // 선택 색상 적용
    public void UpdateVisualState()
    {
        var cb = button.colors;
        if (isSelected)
            cb.normalColor = cb.selectedColor;
        else
            cb.normalColor = Color.white;
       button.colors = cb;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_manager != null)
            _manager.HighlightSameTypeToLeft(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_manager != null)
            _manager.HighlightSameTypeToLeft(this, false);
    }
}
