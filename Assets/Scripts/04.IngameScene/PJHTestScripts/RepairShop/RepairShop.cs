using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
public class RepairShop : MonoBehaviour
{
    [SerializeField] private GameObject RepairShopUI;
    [SerializeField] public List<GameObject> RepairShopUpperTabs;
    [SerializeField] public List<GameObject> RepairShopLowerTabs;
    
    public int currentMoney;
    public TMP_Text currentMoneyText;
    // Start is called before the first frame update
    void Start()
    {
        currentMoney = 30000;
        currentMoneyText.text = currentMoney.ToString();
    }

    string SetMoneyText(int Money)
    {
        string format = $"{currentMoney} - {Money} = {currentMoney - Money}";
        if (Money <= currentMoney)
        {
            return format;
        }
        return currentMoney.ToString();
        
    }
    // Update is called once per frame
   
}
