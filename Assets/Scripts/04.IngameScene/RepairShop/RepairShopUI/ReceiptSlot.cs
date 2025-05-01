//1
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReceiptSlot : MonoBehaviour
{
    [SerializeField] public Image _icon;
    [SerializeField] public TextMeshProUGUI _name;
    [SerializeField] public GameObject _checkbox;
    [SerializeField] public int Index = -1;
    [SerializeField] public int ID = -1;
    [SerializeField] public int type;
    [SerializeField] private RepairShopReceipt manager;
    
    public void OnClick()
    {
        // 유니티 기본 선택 기능 해제
        EventSystem.current.SetSelectedGameObject(null);
        manager.ReceiptUndo(this, Index);
        manager.RepairShop.SetDescription("","");
    }
}

