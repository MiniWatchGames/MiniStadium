using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHud : MonoBehaviour
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
    
    private DetectPlayerStateChanged _detectStat;
    public PlayerController playerStat;
    // Start is called before the first frame update
    void Start()
    {
        playerStat = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        _detectStat = new DetectPlayerStateChanged(playerStat);
        
        playerHPBar = GameObject.Find("[Image] PlayerHpSlider");
        playerHPText = GameObject.Find("[Text] PlayerHp");
        playerName = GameObject.Find("[Text] PlayerName");
        playerWeapon = GameObject.Find("[GroupLayer] PlayerWeapon");
        playerWeaponAmmo = playerWeapon.transform.GetChild(1).gameObject;
        UpdateUI();
        _detectStat.PropertyChanged += OnDetectPlayerStatChanged;
    }

    public void FixedUpdate()
    {
        if (playerStat != null && _detectStat != null)
        {
            _detectStat.playerHp = playerStat.CurrentHp;
            _detectStat.playerMaxHp = playerStat.BaseMaxHp;
        }
    }

    // public void Update()
    // {
    //     
    // }
    // Update is called once per frame
    private void OnDetectPlayerStatChanged(object sender, PropertyChangedEventArgs e)
    {
        //Debug.Log("functioning");
        if(e.PropertyName == "playerHp")
        {
            playerHPBar.GetComponent<Image>().fillAmount = _detectStat.playerHp / _detectStat.playerMaxHp;
            playerHPText.GetComponent<TMP_Text>().text = $"{_detectStat.playerHp.ToString()}|{_detectStat.playerMaxHp.ToString()}";

        }
        if(e.PropertyName == "playerName")
        {
            playerName.GetComponent<TMP_Text>().text = _detectStat.playerName;
        }
        
    }
    private void UpdateUI()
    {
        playerHPBar.GetComponent<Image>().fillAmount = _detectStat.playerHp / _detectStat.playerMaxHp;
        playerHPText.GetComponent<TMP_Text>().text = $"{_detectStat.playerHp.ToString()}|{_detectStat.playerMaxHp.ToString()}";
        playerName.GetComponent<TMP_Text>().text = _detectStat.playerName;
    }

    
}
