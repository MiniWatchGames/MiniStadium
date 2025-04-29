using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InGameRoundUI : MonoBehaviour
{
    [SerializeField] private GameObject RoundCount;
    //[SerializeField] private GameObject RoundTime;
    [SerializeField] private GameObject BlueWinCount;
    [SerializeField] private GameObject RedWinCount;
    [SerializeField] private InGameManager inGameManager;
    
    private InGameUIDetect inGameUIDetect;

    void Start()
    {
        inGameManager = FindObjectsByType<InGameManager>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];
        inGameUIDetect = new InGameUIDetect(inGameManager);
        inGameUIDetect.PropertyChanged += OnIngameChanged;
        inGameManager.inGameUIAction = () => UIUpdate();
        InitUI();
    }

    public void UIUpdate()
    {
        
        //inGameUIDetect.GameTime = inGameManager.timer;
        inGameUIDetect.GameRound = inGameManager.currentRound;
        inGameUIDetect.BlueWinCount = inGameManager.BlueWinCount;
        inGameUIDetect.RedWinCount = inGameManager.RedWinCount;
        Debug.Log("UIUpdate");
    }
    private void OnIngameChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.Log("functioning");
        switch (e.PropertyName)
        {
            case "GameRound":
                RoundCount.GetComponent<TMP_Text>().text = inGameUIDetect.GameRound.ToString() + "R";
                break;
            case "BlueWinCount":
                BlueWinCount.GetComponent<TMP_Text>().text = inGameUIDetect.BlueWinCount.ToString();
                break;
            case "RedWinCount":
                RedWinCount.GetComponent<TMP_Text>().text = inGameUIDetect.RedWinCount.ToString();
                break;
        }
    }

    private void InitUI()
    {
        RoundCount.GetComponent<TMP_Text>().text = inGameUIDetect.GameRound.ToString() + "R";
        BlueWinCount.GetComponent<TMP_Text>().text = inGameUIDetect.BlueWinCount.ToString();
        RedWinCount.GetComponent<TMP_Text>().text = inGameUIDetect.RedWinCount.ToString();
        UIUpdate();
    }
}
