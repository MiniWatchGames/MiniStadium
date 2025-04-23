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
    
    private DetectInGameInfoChanged _detectStat;
    public TestStat playerStat;
    // Start is called before the first frame update
    void Start()
    {
        _detectStat = new DetectInGameInfoChanged(playerStat);
        UpdateUI();
        _detectStat.PropertyChanged += OnDetectPlayerStatChanged;
    }

    public void FixedUpdate()
    {
        Debug.Log("Update" + _detectStat.playerHp);
        _detectStat.playerHp = playerStat.health;
        _detectStat.playerMaxHp = playerStat.maxHealth;
    }

    // public void Update()
    // {
    //     
    // }
    // Update is called once per frame
    private void OnDetectPlayerStatChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.Log("functioning");
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
