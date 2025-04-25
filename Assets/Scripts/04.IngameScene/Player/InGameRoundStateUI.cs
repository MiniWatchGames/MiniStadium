using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class InGameRoundStateUI : MonoBehaviour
{
    [SerializeField] private GameObject RoundCount;
    [SerializeField] private GameObject RoundTime;
    [SerializeField] private GameObject BlueWinCount;
    [SerializeField] private GameObject RedWinCount;
    [SerializeField] private InGameManager inGameManager;
    
    private InGameUIDetect inGameUIDetect;

    void Start()
    {
        inGameUIDetect = new InGameUIDetect(inGameManager);
        inGameUIDetect.PropertyChanged += OnIngameChanged;
    }
    private void OnIngameChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.Log("functioning");
        switch (e.PropertyName)
        {
            case "GameRound":
                RoundCount.GetComponent<TMP_Text>().text = inGameUIDetect.GameRound.ToString();
                break;
            case "BlueWinCount":
                BlueWinCount.GetComponent<TMP_Text>().text = inGameUIDetect.BlueWinCount.ToString();
                break;
            case "RedWinCount":
                RedWinCount.GetComponent<TMP_Text>().text = inGameUIDetect.RedWinCount.ToString();
                break;
        }
    }
}
