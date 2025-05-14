using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudPassives : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] public Image icon;
    [SerializeField] public Image activeRingImage;
    [SerializeField] public Image mask;
    [SerializeField] public float coolTime;
    
    public void TurnVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
