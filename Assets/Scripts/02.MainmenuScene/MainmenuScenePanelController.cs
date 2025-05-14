using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuScenePanelController : PanelController
{
    public Button gameStartButton;
    public Button createServerButton;
    public Button rankingButton;
    public Button settingsButton;
    public Button exitButton;

    public Button goMainmenu;

    private GameObject _playerManager;

    private void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartClicked);
        createServerButton.onClick.AddListener(OnCreateServerClicked);
        rankingButton.onClick.AddListener(OnRankingClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        goMainmenu.onClick.AddListener(OnGoMainMenuClicked);
        InstanceFinder.ServerManager.OnServerConnectionState += HandleServerConnectionState;
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -= HandleServerConnectionState;
    }
    private void HandleServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("playerManager1");
            if (_playerManager == null)
            {
                Debug.Log("playerManager2");
                _playerManager = Instantiate(Resources.Load<GameObject>("Prefabs/Defaults/Server/PlayerManager"));
                InstanceFinder.ServerManager.Spawn(_playerManager);
                Debug.Log("서버에 playerManager가 추가됨");
            }
        }
        StartCoroutine(WaitForPlayerManager());
    }
    
    private IEnumerator WaitForPlayerManager()
    {
        PlayerManager pm = null;
        while ((pm = FindObjectOfType<PlayerManager>()) == null)
            yield return null;

        while (!pm.IsSpawned)
            yield return null;
        
        _playerManager = pm.gameObject;
        SceneManager.LoadScene("MatchingScene");
    }

    private void OnGameStartClicked()
    {
        GetComponent<NetworkMaker>().OnClick_Client();
        SceneManager.LoadScene("MatchingScene");
    }

    private void OnCreateServerClicked()
    {
        GetComponent<NetworkMaker>().OnClick_Server();
    }

    public void OpenPanel()
    {
        
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

    private void OnGoMainMenuClicked()
    {
        ClosePanel("[PopUpPanel] Result");
    }
}
