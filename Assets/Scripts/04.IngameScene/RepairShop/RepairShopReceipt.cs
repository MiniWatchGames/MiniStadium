using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

  	[SerializeField] public RepairShop RepairShop;
    [SerializeField] private RepairShopWeapon RepairShopWeapon;
    [SerializeField] private RepairShopSkill RepairShopSkill;
    [SerializeField] public PlayerHud PlayerHud;
    public PlayerItems PlayerItems = new PlayerItems();

    // UI
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Sprite noneSprite;
    public ReceiptSlot[][] receiptSlots; // 0: 이동기, 1: 전용기, 2: 패시브, 3: 무기
    private string[] _defaultNames = { "이동기", "무기 전용기", "패시브", "기본 무기" };

    // 스테이터스 관련
    [SerializeField] public GameObject status;
    [SerializeField] public GameObject statusGroup;
    [SerializeField] public int Count_HP = 0;
    [SerializeField] public int Count_AR = 0;
    [SerializeField] public int Count_MV = 0;
    [SerializeField] public int Count_JP = 0;
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

    private void Start()
    {
        FromReceiptToHud();
    }

    // 현재 구매 된 아이템 정보 PlayerItems에 기록
    public void SetPlayerItems()
    {
        // 무기
        var weapon = RepairShopWeapon.currentWeapon;
        if (weapon == null) weapon = RepairShopWeapon.baseWeapon;
        PlayerItems.weapon_Type = weapon.type;
        PlayerItems.weapon_ID = weapon.index;
        PlayerItems.weapon_Name = weapon.nameText.text;

        // 스테이터스
        PlayerItems.count_HP = Count_HP;
        PlayerItems.count_AR = Count_AR;
        PlayerItems.count_MV = Count_MV;
		PlayerItems.count_JP = Count_JP;

        // 스킬
        for (int i = 0; i < 3; i++)
        {
            // 3개의 튜플 초기화
            PlayerItems.Skills[i] = new (int, string)[3];
            var j = 0;

            foreach (var slot in receiptSlots[i])
            {
                if (receiptSlots[i] != null && slot._checkbox.activeSelf)
                {
                    if (slot.ID == 0 || slot._name.text == "") continue;
                    PlayerItems.Skills[i][j] = new(slot.ID, slot._name.text);
                    j++;
                }
            }
        }
        
        // Debug.Log($"Weapon : {PlayerItems.weapon_Name}");
        // Debug.Log($"HP: {PlayerItems.count_HP} AR: {PlayerItems.count_AR} MV: {PlayerItems.count_MV} JP: {PlayerItems.count_JP}");
        // Debug.Log($"Skill 00 : {Dump(PlayerItems.Skills[0][0])}, Skill 01 : {Dump(PlayerItems.Skills[0][1])}, Skill 02 : {Dump(PlayerItems.Skills[0][2])}");
        // Debug.Log($"Skill 10 : {Dump(PlayerItems.Skills[1][0])}, Skill 11 : {Dump(PlayerItems.Skills[1][1])}, Skill 12 : {Dump(PlayerItems.Skills[1][2])}");
        // Debug.Log($"Skill 20 : {Dump(PlayerItems.Skills[2][0])}, Skill 21 : {Dump(PlayerItems.Skills[2][1])}, Skill 22 : {Dump(PlayerItems.Skills[2][2])}");
    }
    
    // private string Dump((int, string) tuple)
    //     {
    //         return tuple == default ? "null" : $"{tuple.Item1}:{tuple.Item2}";
    //     }

    
    // HUD 상의 정보 갱신
    public void FromReceiptToHud()
    {
        PlayerHud.Update_HUD_Comp( RepairShopWeapon.currentWeapon == null ? 
                RepairShopWeapon.baseWeapon : RepairShopWeapon.currentWeapon, 
            receiptSlots);
    }
    
    // receipt 내의 스테이터스 색상 처리
    public void CopyStatusColor(int i, int j, RepairShopStatusButton button)
    {
        var cb = button.GetComponent<Button>().colors;

        PreviewsInRows[i, j].color = button.isBought ? cb.disabledColor : cb.normalColor;
    }
    
    // 모두 초기화
    public void ReceiptRefundAll()
    {
        PlayerItems.weapon_Type = -1;
        PlayerItems.weapon_ID = -1;
        PlayerItems.weapon_Name = null;
        
        Count_HP = 0;
        Count_AR = 0;
        Count_MV = 0;
 		Count_JP = 0;

        for (int i = 0; i < receiptSlots.Length; i++)
        {
            foreach (var slot in receiptSlots[i])
                ResetTargetSlot(slot, i);
            
            if (i >= 3) continue;
            PlayerItems.Skills[i] = null;
        }
        
        // null 참조 방지
        SetPlayerItems();
    }

    // 클릭 시 선택 취소
    public void ReceiptUndo(ReceiptSlot slot, int index)
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
           
            if (RepairShopWeapon.selectedWeapon != null && RepairShopWeapon.selectedWeapon != RepairShopWeapon.currentWeapon)
            {
                RepairShop.totalPrice -= RepairShopWeapon.selectedWeapon.price;
                RepairShopWeapon.selectedWeapon.Selected(false);
                RepairShopWeapon.selectedWeapon = null;
                RepairShop.UpdateMoneyText(RepairShop.totalPrice);
                ResetTargetSlot(slot, 3);
  				RepairShopSkill.SetWeaponSkillUI(-1);
                ReceiptUndo(receiptSlots[1][0], 0);
                ReceiptUndo(receiptSlots[1][1], 1);
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
    
    // 항목 갱신
    public void ReceiptUpdateSlot(bool buying, int type)
    {
        var shopSkills = buying ? RepairShopSkill.boughtSkills : RepairShopSkill.selectedSkills;
        
        if (type >= receiptSlots.Length)
        {
            Debug.LogWarning($"ReceiptUpdateSkill: 잘못된 타입 인덱스 {type}");
            return;
        }
        
        // 무기 구매
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
        // 스킬 구매
        if (buying)
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
