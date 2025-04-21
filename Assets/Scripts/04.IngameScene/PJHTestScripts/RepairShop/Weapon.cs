using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Weapon : MonoBehaviour
{
    public Items buyableWeapon;

    private GameObject _weapon;
    
    // Start is called before the first frame update
    public void init()
    {
        _weapon = gameObject;
        Debug.Log(_weapon.name);
        
        foreach(Transform UI in _weapon.GetComponentsInChildren<Transform>())
        {
            // if (UI.name.Contains("Icon"))
            // {
            //     UI.GetComponent<Image>().sprite = buyableWeapon.icon;
            // }
            // if (UI.name.Contains("Name"))
            // {
            //     UI.GetComponent<TMP_Text>().text = buyableWeapon.name;
            // }
            // if (UI.name.Contains("Price"))
            // {
            //     UI.GetComponent<TMP_Text>().text = buyableWeapon.price.ToString();
            // }
        }
        
    }

   
}
