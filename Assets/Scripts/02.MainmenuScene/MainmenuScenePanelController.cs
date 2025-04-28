using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuScenePanelController : PanelController
{
    public Button gameStartButton;
    public Button rankingButton;
    public Button settingsButton;
    public Button exitButton;

    private void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartClicked);
        rankingButton.onClick.AddListener(OnRankingClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnGameStartClicked()
    {
        SceneManager.LoadScene("MatchingScene");
    }

    private void OnRankingClicked()
    {
        OpenPanel("[PopUpPanel] Ranking");
    }

    private void OnSettingsClicked()
    {
        OpenPanel("[PopUpPanel] Settings");
    }

    public void CloseRankingPanel()
    {
        ClosePanel("[PopUpPanel] Ranking");
    }

    private void CloseSettingsPanel()
    {
        ClosePanel("[PopUpPanel] Settings");
    }

    private void OnExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
