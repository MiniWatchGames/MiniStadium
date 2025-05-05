using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    
    [SerializeField] private GameObject playerSkill1;
    [SerializeField] private GameObject playerSkill2;
    [SerializeField] private GameObject playerSkill3;
    [SerializeField] private GameObject playerWeapon;
    [SerializeField] private GameObject playerWeaponAmmo;

    private float currentHp = 0;
    private   float maxHp = 0;
    private DetectPlayerStateChanged _detectStat;
    public PlayerController playerStat;
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
