using System.Collections;
using System.Collections.Generic;
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
    
    int playerMaxHp = 100;
    int playerMaxArmor = 100;
    float playerHp = 100;
    // Start is called before the first frame update
    void Start()
    {
        SetPlayerHP();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Player HP: " + playerHp);
    }
    public void SetPlayerHP()
    {
        string HpFormat = $"{playerHp} | {playerMaxHp}";
        Debug.Log(HpFormat);
        playerHPBar.GetComponent<Image>().fillAmount = (playerHp)/playerMaxHp;
        playerHPText.GetComponent<TMP_Text>().text = HpFormat;
    }

    public void PlayerDamaged(float damage)
    {  
        Debug.Log("Player Damaged: " + damage);
        playerHp -= damage;
        string HpFormat = $"{playerHp} | {playerMaxHp}";
        playerHPBar.GetComponent<Image>().fillAmount = (playerHp)/playerMaxHp;
        playerHPText.GetComponent<TMP_Text>().text = HpFormat;
    }
    public void PlayerHeals(float damage)
    {   
        playerHp += damage;
        string HpFormat = $"{playerHp} | {playerMaxHp}";
        playerHPBar.GetComponent<Image>().fillAmount = (playerHp)/playerMaxHp;
        playerHPText.GetComponent<TMP_Text>().text = HpFormat;
    }
}
