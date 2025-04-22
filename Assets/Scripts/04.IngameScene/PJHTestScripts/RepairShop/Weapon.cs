using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{

    public ItemInfo item;
    private GameObject _weapon;
    Action<float> onWeaponClickedAction;
    // Start is called before the first frame update
    public void init(ItemInfo itemInfo, Action<float> onClickAction = null)
    {
        _weapon = gameObject;
        item = itemInfo;
        Debug.Log(_weapon.name);
        onWeaponClickedAction = onClickAction;
        foreach(Transform UI in _weapon.GetComponentsInChildren<Transform>())
        {
            if (UI.name.Contains("Icon"))
            {
                UI.GetComponent<Image>().sprite = itemInfo.icon;
            }
            if (UI.name.Contains("Name"))
            {
                UI.GetComponent<TMP_Text>().text = itemInfo.name;
            }
            if (UI.name.Contains("Price"))
            {
                UI.GetComponent<TMP_Text>().text = itemInfo.price.ToString() + "g";
            }
        }
    }

    public void OnClick(Action onClickAction)
    {
        onWeaponClickedAction?.Invoke(item.price);
    }
    

   
}
