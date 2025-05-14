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
    [SerializeField] private PlayerHudComps passive0;
    [SerializeField] private PlayerHudComps passive1;
    [SerializeField] private PlayerHudComps passive2;
    [SerializeField] private PlayerHudComps playerWeapon;
    [SerializeField] private GameObject playerWeaponAmmo;
    [SerializeField] private GameObject skillGage;

    private float maxAmmo;
    private float currentAmmo;
    private float currentHp = 0;
    private float maxHp = 0;
    private DetectPlayerStateChanged _detectStat;
    public PlayerController playerStat;
    [SerializeField] private IWeapon weaponInfo;
    // Start is called before the first frame update
    public void init(PlayerController player)
    {
        playerStat = player;
        _detectStat = new DetectPlayerStateChanged(playerStat);
        weaponInfo = null;
        
        //playerHPBar = GameObject.Find("[Image] PlayerHpSlider");
        //playerHPText = GameObject.Find("[Text] PlayerHp");
        playerName.GetComponent<TextMeshProUGUI>().text = playerStat.name;
        //playerWeapon = GameObject.Find("[GroupLayer] PlayerWeapon");
        //playerWeaponAmmo = playerWeapon.transform.GetChild(1).gameObject;
        //UpdateUI();
        //_detectStat.PropertyChanged += OnDetectPlayerStatChanged;

        weaponInfo = playerStat.CombatManager.CurrentWeapon.GetComponent<IWeapon>();
        if (weaponInfo.CurrentAmmo != null) {
            weaponInfo.CurrentAmmo.AddObserver(this);
            currentAmmo = weaponInfo.CurrentAmmo.Value; }
        if (weaponInfo.MaxAmmo != null) {
            weaponInfo.MaxAmmo.AddObserver(this);
            maxAmmo = weaponInfo.MaxAmmo.Value; 
            playerWeapon.text.text = $"{currentAmmo} | {maxAmmo}";
        }
        playerStat.CurrentFirstMovementSkillCoolTime.AddObserver(this);
        playerStat.CurrentSecondMovementSkillCoolTime.AddObserver(this);
        playerStat.CurrentFirstWeaponSkillCoolTime.AddObserver(this);
        playerStat.CurrentSecondWeaponSkillCoolTime.AddObserver(this);
        
        playerStat.CurrentHp.AddObserver(this);
        playerStat.BaseMaxHp.AddObserver(this);
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
        ReceiptSlot[][] receipts)
    {
        PlayerHudComps[][] comps = 
            { new PlayerHudComps[1], 
                new PlayerHudComps[2], 
                new PlayerHudComps[3], 
                new PlayerHudComps[1]};
        comps[0][0] = moveSkill0;
        comps[1][0] = weaponSkill0; 
        comps[1][1] = weaponSkill1;
        comps[2][0] = passive0; 
        comps[2][1] = passive1; 
        comps[2][2] = passive2;
        comps[3][0] = playerWeapon; 


        for (int i = 0; i < comps.Length; i++)
        {
            for (int j = 0; j < comps[i].Length; j++)
            {
                var comp = comps[i][j];
                // 무기
                if (i == 0 && currentWeapon != null)
                {
                    comp.icon.sprite = currentWeapon.iconImage.sprite;
                    if (currentWeapon.type == 2)
                    {
                        comp.text.text = "\u221e";
                    }
                }
                //스킬
                else
                {
                    if (receipts[i][j] == null || receipts[i][j].ID == -1)
                    {
                        comp.TurnVisibility(false);
                    }
                    else
                    {
                        comp.TurnVisibility(true);
                        comp.icon.sprite = receipts[i][j]._icon.sprite;
                        if (comp.text != null)
                            comp.text.text = ""; // 쿨타임
                    }
                
                }
                comp.coolTime = 0;
                if (comp.mask != null)
                    comp.mask.fillAmount = 0;
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
        else if (data.Item2 == "GunMaxAmmo")
        {
            maxAmmo = data.Item1;
            playerWeapon.text.text = $"{currentAmmo} | {maxAmmo}";
            return;
        }
        else if (data.Item2  == "GunCurrentAmmo")
        {
            currentAmmo = data.Item1;
            playerWeapon.text.text = $"{currentAmmo} | {maxAmmo}";
            return;
        }
        else if (data.Item2  == "currentFirstMovementSkillCoolTime")
        {
            //moveSkill0.mask.fillAmount = Mathf.Lerp(0, 1, data.Item1/);
            HudCoolTimer(data.Item1, moveSkill0);
            return;
        }
        else if (data.Item2  == "currentSecondMovementSkillCoolTime")
        {
            return;
        }
        else if (data.Item2  == "currentFirstWeaponSkillCoolTime")
        {
            HudCoolTimer(data.Item1, weaponSkill0);
            return;
        }
        else if (data.Item2  == "currentSecondWeaponSkillCoolTime")
        {
            HudCoolTimer(data.Item1, weaponSkill1);
            return;
        }
        playerHPBar.GetComponent<Image>().fillAmount = currentHp / maxHp;
        playerHPText.GetComponent<TMP_Text>().text = $"{currentHp.ToString()}|{maxHp.ToString()}";
    }
    
    private void HudCoolTimer(float count, PlayerHudComps comp)
    {
        if (count > comp.coolTime)
            comp.coolTime = count;
        
        comp.mask.fillAmount = Mathf.Lerp(0, 1, count/comp.coolTime);
        
        if (count >= 1)
        {
            comp.text.text = count.ToString("F0");
        }
        else if (count < 0.1)
        {
            comp.text.text = "";
        }
        else
        {
            comp.text.text = $".{(count * 10):F0}";
        }
    }
}
