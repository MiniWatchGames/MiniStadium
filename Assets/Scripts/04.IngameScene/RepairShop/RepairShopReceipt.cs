using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairShopReceipt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private RepairShopWeapon Weapon;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Sprite noneSprite;
    [SerializeField] private GameObject skill;
    [SerializeField] private List<GameObject> skills;
    [SerializeField] private Transform skillParent;
    [SerializeField] public GameObject status;
    [SerializeField] public GameObject statusGroup;
    public Image[,] previewsInRows;

    void Start()
    {
        skills = new List<GameObject>();
    }
    
    public void ChangeStatusColor(int i, int j, Color color)
    {
        previewsInRows[i, j].color = color;
    }
    
    public void ReceiptRefundWeapon()
    {
        weaponImage.sprite = noneSprite;
        weaponText.text = "무기 없음";
    }

    public void ReceiptRefundSkill()
    {
        foreach (var skill in skills)
            Destroy(skill);
        skills.Clear();
    }
    
    public void ReceiptBuyWeapon()
    {
        weaponText.text = Weapon.currentWeapon.nameText.text;
        weaponImage.sprite = Weapon.currentWeapon.iconImage.sprite;
    }

    public void ReceiptBuySkill(string text, Sprite sprite)
    {
        var skil = Instantiate(skill, skillParent);
        skil.GetComponent<ReceiptSkillSlot>().skillName.text = text;
        skil.GetComponent<ReceiptSkillSlot>().skillIcon.sprite = sprite;
        skills.Add(skil);
    }
}
