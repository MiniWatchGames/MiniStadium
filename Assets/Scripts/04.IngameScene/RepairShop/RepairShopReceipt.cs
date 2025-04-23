using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepairShopReceipt : MonoBehaviour
{
    [SerializeField] private RepairShopWeapon RepairShopWeapon;
    [SerializeField] private RepairShopSkill RepairShopSkill;
    
    // UI
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Sprite noneSprite;
    [SerializeField] private List<ReceiptSlot> receiptSlots;
    
    // 스테이터스 관련
    [SerializeField] public GameObject status;
    [SerializeField] public GameObject statusGroup;
    public Image[,] PreviewsInRows;
    private string[] _defaultNames = { "이동기", "무기 전용기", "패시브", "무기 없음" };
    
    
    ///TODO:  스테이터스 미리보기 적용
    /// 
    
    // 스테이터스 색상 처리
    public void ChangeStatusColor(int i, int j, Color color)
    {
        PreviewsInRows[i, j].color = color;
    }
    
    // 모두 초기화
    public void ReceiptRefundAll()
    {
        for (int i = 0; i < receiptSlots.Count; i++)
        {
            var target = receiptSlots[i];
            target._icon.GetComponent<Button>().interactable = true;
            target._checkbox.SetActive(false);
            
            target._icon.sprite = noneSprite;
            target._name.text = _defaultNames[i];
        }
    }

    // 클릭 시 선택 취소
    public void ReceiptUndo(int type)
    {
        // 타입 유효 체크, type: 0=이동기, 1=전용기, 2=패시브, 3=무기
        if (type < 0 || type >= receiptSlots.Count)
        {
            Debug.LogWarning($"ReceiptUndo: 잘못된 타입 인덱스 {type}");
            return;
        }
        
        var target = receiptSlots[type];
        
        // 반환 유효 체크 (임시)
        if (target._icon.sprite == noneSprite)
            return;
        
        // 항목 초기화
        if (type < _defaultNames.Length)
            target._name.text = _defaultNames[type];
        else
            target._name.text = "?";
        
        target._icon.sprite = noneSprite;

        // 무기 선택 해제
        if (type == 3)
            RepairShopWeapon.ReadWeaponInfo(RepairShopWeapon.selectedWeapon);
        // 스킬 선택 해제
        else
            RepairShopSkill.ReadSkillInfo(RepairShopSkill.selectedSkills[type]);
    }
    
    // 항목 갱신 및 구매
    public void ReceiptUpdateSkill(bool buying)
    {
        RepairShopSkillSlot[] skills;
        RepairShopWeaponSlot weapon;

        if (buying)
        {
            skills = RepairShopSkill.boughtSkills;
            weapon = RepairShopWeapon.currentWeapon;
        }
        else
        {
            skills = RepairShopSkill.selectedSkills;
            weapon = RepairShopWeapon.selectedWeapon;
        }
        
        for (int i = 0; i < receiptSlots.Count; i++)
        {
            var slotName = receiptSlots[i]._name;
            var slotIcon = receiptSlots[i]._icon;
            
            // 무기 처리
            if (i == 3 && weapon != null)
            {
                slotName.text = weapon.nameText.text;
                slotIcon.sprite = weapon.iconImage.sprite;
                // 구매 처리
                if (!buying) continue;
                receiptSlots[i]._checkbox.SetActive(true);
                slotIcon.GetComponent<Button>().interactable = false;
            }
            // 스킬 처리
            if (i >= skills.Length || skills[i] == null) continue;
            slotName.text = skills[i].nameText.text;
            slotIcon.sprite = skills[i].iconImage.sprite;
            // 구매 처리
            if (!buying) continue;
            receiptSlots[i]._checkbox.SetActive(true);
            slotIcon.GetComponent<Button>().interactable = false;
        }
    }
}
