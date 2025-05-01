// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// public class RepairShopStatusButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
// {
//     public BuyableObject_Status boStatus;
//     
//     // 타입 0= 체력, 1= 방어력, 2= 이동속도
//     public int StatusType => _statusType;
//     private int _statusType;
//     // 버튼 순서
//     public int StatusIndex => _statusIndex;
//     private int _statusIndex;
//     // 가격
//     public int StatusPrice => _price;
//     private int _price;
//
//     private RepairShopStatus _manager;
//     
//     // 버튼 상태 관련
//     public bool isSelected = false;
//     public bool isBought = false;
//     public Button button;
//     
//     public string description;
//
//     // 정보 설정
//     public void Init(int index, RepairShopStatus manager
//         , int originalPrice, int addtivePrice, BuyableObject_Status bo)
//     {
//         _statusIndex = index;
//         _manager = manager;
//         boStatus = bo;
//         button = GetComponent<Button>();
//         _statusType = boStatus.type;
//         
//         description = bo.description;
//         
//         // 업그레이드 레벨 당 가격 (addtivePrice)g증가
//         if (index == 0)
//             _price = 0;
//         else
//             _price = originalPrice + (index - 1) * addtivePrice;
//
//         var buttonColors = button.colors;
//         buttonColors.disabledColor = boStatus.DisabledColor;
//         buttonColors.highlightedColor = boStatus.HighlightedColor;
//         buttonColors.pressedColor = boStatus.PressedColor;
//         buttonColors.selectedColor = boStatus.SelectedColor;
//         button.colors = buttonColors;
//     }
//
//     public void OnClick()
//     {
//         // 좌측의 모든 버튼들을 선택
//         _manager.SelectSameTypeToLeft(this);
//         // 유니티 기본 선택 기능 해제
//         EventSystem.current.SetSelectedGameObject(null);
//     }
//
//     // 선택 시 비주얼 처리 및 가격정보 전달
//     public void SetSelected(bool selected)
//     {
//         isSelected = selected;
//         UpdateVisualState();
//     }
//     
//     // 선택 색상 적용
//     public void UpdateVisualState()
//     {
//         var cb = button.colors;
//         cb.normalColor = isSelected ? cb.selectedColor : Color.white;
//         button.colors = cb;
//     }
//
//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         _manager.HighlightSameTypeToLeft(this, true);
//     }
//
//     public void OnPointerExit(PointerEventData eventData)
//     {
//         _manager.HighlightSameTypeToLeft(this, false);
//     }
// }
