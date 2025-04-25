using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RepairShopReceipt : MonoBehaviour
{
    [Serializable]
    public class ReceiptSlotRow
    {
        public ReceiptSlot[] row;
    }
    [SerializeField] private ReceiptSlotRow[] receiptSlotRows;

    [SerializeField] private RepairShop RepairShop;
    [SerializeField] private RepairShopWeapon RepairShopWeapon;
    [SerializeField] private RepairShopSkill RepairShopSkill;
    public PlayerItems PlayerItems = new PlayerItems();

    // UI
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Sprite noneSprite;
    public ReceiptSlot[][] receiptSlots; // 0: 이동기, 1: 전용기, 2: 패시브, 3: 무기
    private string[] _defaultNames = { "이동기", "무기 전용기", "패시브", "무기 없음" };

    // 스테이터스 관련
    [SerializeField] public GameObject status;
    [SerializeField] public GameObject statusGroup;
    [SerializeField] public int Count_HP = 0;
    [SerializeField] public int Count_AR = 0;
    [SerializeField] public int Count_MV = 0;
    public Image[,] PreviewsInRows;
    
    private void Awake()
    {
        int rowCount = receiptSlotRows.Length;
        receiptSlots = new ReceiptSlot[rowCount][];
        for (int i = 0; i < rowCount; i++)
        {
            receiptSlots[i] = receiptSlotRows[i].row;
        }
    }
    
    ///TODO:  스테이터스 미리보기 적용
    /// 
    
    // 현재 구매 된 아이템 정보 PlayerItems에 기록
    public void SetPlayerItems()
    {
        // 무기
        var weapon = RepairShopWeapon.currentWeapon;
        if (weapon != null)
        {
            PlayerItems.weapon_Type = weapon.type;
            PlayerItems.weapon_Index = weapon.index;
            PlayerItems.weapon_Name = weapon.nameText.ToString();
        }

        // 스테이터스
        PlayerItems.count_HP = Count_HP;
        PlayerItems.count_AR = Count_AR;
        PlayerItems.count_MV = Count_MV;

        // 스킬
        for (int i = 0; i < 3; i++)
        {
            // Skills[i]가 null이면 초기화
            if (PlayerItems.Skills[i] == null)
            {
                PlayerItems.Skills[i] = new Dictionary<int, string>();
            }

            foreach (var slot in receiptSlots[i])
            {
                if (slot._checkbox.activeSelf)
                {
                    // 중복되지 않도록 TryAdd()를 사용
                    PlayerItems.Skills[i].TryAdd(slot.type, slot._name.text);
                }
            }
        }
    }
    
    // 스테이터스 색상 처리
    public void ChangeStatusColor(int i, int j, Color color)
    {
        PreviewsInRows[i, j].color = color;
    }
    
    // 모두 초기화
    public void ReceiptRefundAll()
    {
        Count_HP = 0;
        Count_AR = 0;
        Count_MV = 0;

        for (int i = 0; i < receiptSlots.Length; i++)
        {
            foreach (var slot in receiptSlots[i])
            {
                ResetTargetSlot(slot, i);
            }
            
            if (i >= 3) continue;
            PlayerItems.Skills[i] = null;
        }
    }

    // 클릭 시 선택 취소
    public void ReceiptUndo(ReceiptSlot slot, int index, int ID)
    {
        var type = slot.type;
        
        // 타입 유효 체크, type: 0=이동기, 1=전용기, 2=패시브, 3=무기
        if (type < 0 || type >= receiptSlots.Length)
        {
            Debug.LogWarning($"ReceiptUndo: 잘못된 타입 인덱스 {type}");
            return;
        }
        
        // 무기 선택 해제
        if (type == 3)
        {
            // 선택 해제
            if (RepairShopWeapon.selectedWeapon != null && RepairShopWeapon.selectedWeapon != RepairShopWeapon.currentWeapon)
            {
                RepairShop.totalPrice -= RepairShopWeapon.selectedWeapon.price;
                RepairShopWeapon.selectedWeapon.Selected(false);
                RepairShopWeapon.selectedWeapon = null;
                RepairShop.UpdateMoneyText(RepairShop.totalPrice);
                ResetTargetSlot(slot, 3);
            }
            return;
        }
        
        // 스킬 선택 해제
        if (type < RepairShopSkill.selectedSkills.Length)
        {
            var selected = RepairShopSkill.selectedSkills[type];
            if (index >= 0 && index < selected.Length && selected[index] != null)
            {
                var skill = selected[index];
                
                if (!skill.isBought)  // 구매되지 않은 경우에만 선택 해제 가능
                {
                    RepairShop.totalPrice -= skill.price;
                    skill.Selected(false);  // 스킬 선택 해제
                    selected[index] = null;

                    RepairShop.UpdateMoneyText(RepairShop.totalPrice);
                    ResetTargetSlot(slot, type);

                    RepairShopSkill.remainingSkillsToBuy[type]++;  // 남은 구매 가능한 수 증가
                }
                else
                {
                    // 이미 구매된 스킬은 선택을 해제할 수 없음
                    Debug.Log("이미 구매한 스킬은 해제할 수 없습니다.");
                }
            }
        }
    }

    // 항목 초기화
    /*
    public void ResetSlot(int type, int index)
    {
        var slot = receiptSlots[type][index];

        if (type == 3) // 무기
        {
            var weapon = RepairShopWeapon.currentWeapon;

            if (weapon != null)
            {
                slot._name.text = weapon.nameText.text;
                slot._icon.sprite = weapon.iconImage.sprite;
                slot.Index = weapon.index;
                slot._checkbox.SetActive(true);
                slot._icon.GetComponent<Button>().interactable = false;
                return;
            }
        }
        else // 스킬
        {
            var skills = RepairShopSkill.boughtSkills;
            var skill = skills[type][index];

            if (skill != null)
            {
                slot._name.text = skill.nameText.text;
                slot._icon.sprite = skill.iconImage.sprite;
                slot.Index = skill.index;
                slot._checkbox.SetActive(true);
                slot._icon.GetComponent<Button>().interactable = false;
                return;
            }
        }

        slot._name.text = _defaultNames[type];
        slot._icon.sprite = noneSprite;
        slot.Index = -1;
    }*/


    // 항목 갱신
    public void ReceiptUpdateSlot(bool buying, int type)
    {
        var shopSkills = buying ? RepairShopSkill.boughtSkills : RepairShopSkill.selectedSkills;
        
        if (type >= receiptSlots.Length)
        {
            Debug.LogWarning($"ReceiptUpdateSkill: 잘못된 타입 인덱스 {type}");
            return;
        }
        
        // 무기 처리
        if (type == 3 || buying)
        {
            var weapon = buying ? RepairShopWeapon.currentWeapon : RepairShopWeapon.selectedWeapon;
            if (weapon != null)
            {
                var slot = receiptSlots[3][0];
                slot._name.text = weapon.nameText.text;
                slot._icon.sprite = weapon.iconImage.sprite;
                slot.ID = weapon.index;
                slot._checkbox.SetActive(buying);
                slot._icon.GetComponent<Button>().interactable = !buying;
            }
        }
        // 스킬 처리
        if (buying) // 구매 시
        {
            for (int i = 0; i < receiptSlots.Length - 1; i++)
            {
                for (int j = 0; j < shopSkills[i].Length; j++)
                {
                    var shopSkill = shopSkills[i][j];
                    var slot = receiptSlots[i][j];

                    if (shopSkill != null)
                    {
                        // 선택된 스킬로 업데이트
                        UpdateSkillSlot(slot, shopSkill, true);
                    }
                }
            }
            SetPlayerItems();
            return;
        }
        if (type < 3) // 선택 처리 (비구매)
        {
            for (int j = 0; j < shopSkills[type].Length; j++)
            {
                var shopSkill = shopSkills[type][j];
                var slot = receiptSlots[type][j];

                if (shopSkill != null)
                {
                    // 구매되지 않은 상태에서 선택된 스킬만 상태 업데이트
                    if (shopSkill.isBought)
                    {
                        // 구매된 스킬은 체크박스 활성화
                        UpdateSkillSlot(slot, shopSkill, true);
                    }
                    else
                    {
                        // 선택된 상태 유지, 체크박스 상태 변경 없음
                        UpdateSkillSlot(slot, shopSkill, false);
                    }
                }
            }
        }
    }
    
    public void ResetTargetSlot(ReceiptSlot slot, int type)
    {
        slot._name.text = _defaultNames[type];
        slot._icon.sprite = noneSprite;
        slot.ID = -1;
        slot._checkbox.SetActive(false);
        slot._icon.GetComponent<Button>().interactable = true;
    }

    private void UpdateSkillSlot(ReceiptSlot slot, RepairShopSkillSlot skill, bool buying)
    {
        slot._name.text = skill.nameText.text;
        slot._icon.sprite = skill.iconImage.sprite;
        slot.ID = skill.index;
        slot._checkbox.SetActive(buying);
        slot._icon.GetComponent<Button>().interactable = !buying;
    }
}
