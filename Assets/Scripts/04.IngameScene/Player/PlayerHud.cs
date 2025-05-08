using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour , IStatObserver
{
    [SerializeField] private GameObject playerHPBar;
    [SerializeField] private GameObject playerHPText;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject playerArmorBar;
    [SerializeField] private GameObject playerArmor;
    
    [SerializeField] private PlayerHudComps moveSkill0;
    [SerializeField] private PlayerHudComps weaponSkill0;
    [SerializeField] private PlayerHudComps weaponSkill1;
    [SerializeField] private PlayerHudComps playerWeapon;
    [SerializeField] private GameObject playerWeaponAmmo;
    [SerializeField] private GameObject skillGage;

    private float currentHp = 0;
    private   float maxHp = 0;
    private DetectPlayerStateChanged _detectStat;
    public PlayerController playerStat;
    private IWeapon _weaponInfo;
    // Start is called before the first frame update
    public void init(PlayerController player)
    {
        playerStat = player;
        _detectStat = new DetectPlayerStateChanged(playerStat);
        
        //playerHPBar = GameObject.Find("[Image] PlayerHpSlider");
        //playerHPText = GameObject.Find("[Text] PlayerHp");
        playerName.GetComponent<TextMeshProUGUI>().text = playerStat.name;
        //playerWeapon = GameObject.Find("[GroupLayer] PlayerWeapon");
        //playerWeaponAmmo = playerWeapon.transform.GetChild(1).gameObject;
        //UpdateUI();
        //_detectStat.PropertyChanged += OnDetectPlayerStatChanged;

        playerStat.CurrentHp.AddObserver(this);
        playerStat.BaseMaxHp.AddObserver(this);
        IWeapon weaponInfo = playerStat.CombatManager.CurrentWeapon.GetComponent<IWeapon>();

        if (weaponInfo.CurrentAmmo != null) { 
            weaponInfo.CurrentAmmo.AddObserver(this);
        }
        if (weaponInfo.MaxAmmo != null) { 
            weaponInfo.MaxAmmo.AddObserver(this);
        }


        currentHp = playerStat.CurrentHp.Value;
        maxHp = playerStat.BaseMaxHp.Value;

        playerHPBar.GetComponent<Image>().fillAmount = currentHp /maxHp;
        playerHPText.GetComponent<TMP_Text>().text = $"{currentHp.ToString()}| {maxHp.ToString()}";
    }

    //public void FixedUpdate()
    //{
    //    if (playerStat != null && _detectStat != null)
    //    {
    //       // _detectStat.playerHp = playerStat.CurrentHp;
    //      // _detectStat.playerMaxHp = playerStat.BaseMaxHp;
    //    }
    //}

    // public void Update()
    // {
    //     
    // }
    // Update is called once per frame
    //private void OnDetectPlayerStatChanged(object sender, PropertyChangedEventArgs e)
    //{
    //    //Debug.Log("functioning");
    //    if(e.PropertyName == "playerHp")
    //    {
    //        playerHPBar.GetComponent<Image>().fillAmount = _detectStat.playerHp / _detectStat.playerMaxHp;
    //        playerHPText.GetComponent<TMP_Text>().text = $"{_detectStat.playerHp.ToString()}|{_detectStat.playerMaxHp.ToString()}";

    //    }
    //    if(e.PropertyName == "playerName")
    //    {
    //        playerName.GetComponent<TMP_Text>().text = _detectStat.playerName;
    //    }
        
    //}
    //private void UpdateUI()
    //{
    //    playerHPBar.GetComponent<Image>().fillAmount = _detectStat.playerHp / _detectStat.playerMaxHp;
    //    playerHPText.GetComponent<TMP_Text>().text = $"{_detectStat.playerHp.ToString()}|{_detectStat.playerMaxHp.ToString()}";
    //    playerName.GetComponent<TMP_Text>().text = _detectStat.playerName;
    //}

    public void Update_HUD_Comp(RepairShopWeaponSlot currentWeapon, 
        ReceiptSlot skill1, ReceiptSlot skill2, ReceiptSlot skill3)
    {
        PlayerHudComps[] comps = 
            { moveSkill0, weaponSkill0, weaponSkill1, playerWeapon };
        ReceiptSlot[] skills = { skill1, skill2, skill3 };

        int i;

        for (i = 0; i < comps.Length; i++)
        {
            UpdateCompUI(comps[i]);
        }
        
        return;
        
        void UpdateCompUI(PlayerHudComps comp)
        {
            if (comp == playerWeapon && currentWeapon != null)
            {
                comp.icon.sprite = currentWeapon.iconImage.sprite;
                if (currentWeapon.type == 2) comp.text.text = "\u221e";
                                        else comp.text.text = "2 | 6";
            }
            else if (i < skills.Length)
            {
                if (skills[i] != null)
                    comp.icon.sprite = skills[i]._icon.sprite;
            }
            
        }
    }
    public GameObject GetSkillGage() {
        return skillGage;
    }

    public void WhenStatChanged((float, string) data)
    {
        if (data.Item2 == "currentHp")
        {
            currentHp = data.Item1;
        }
        else if (data.Item2 == "baseMaxHp")
        {
            maxHp = data.Item1;
        }
        playerHPBar.GetComponent<Image>().fillAmount = currentHp / maxHp;
        playerHPText.GetComponent<TMP_Text>().text = $"{currentHp.ToString()}|{maxHp.ToString()}";
    }
}
