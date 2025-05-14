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
    [Header("[Panel] MainMenu Buttons")]
    [SerializeField] private Button gameStartButton; // 게임시작 버튼
    [SerializeField] private Button rankingButton; // 게임유저 순위보는 버튼 (구현X)
    [SerializeField] private Button createServerButton; // 서버생성 버튼
    [SerializeField] private Button gameSettingButton; // 게임설정 버튼
    [SerializeField] private Button gameExitButton; // 게임종료 버튼

    [Header("[PopupPanel] Settings")]
    [SerializeField] private Button closeSettingButton; // 설정닫기 버튼

    [Header("[PopupPanel] Result")]
    [SerializeField] private GameObject resultPanel; // [PopupPanel] Result 패널
    [SerializeField] private TextMeshProUGUI resultText; // '승리'인지 '패배'인지 나오는 텍스트
    [SerializeField] private TextMeshProUGUI totalRoundText; // 총 라운드 수가 나오는 텍스트
    [SerializeField] private TextMeshProUGUI playerWinText; // 플레이어가 이긴 횟수가 나오는 텍스트
    [SerializeField] private TextMeshProUGUI opponentWinText; // 상대 플레이어가 이긴 횟수가 나오는 텍스트

    private bool isResultActive = false;
    private GameObject _playerManager;

    private void Start()
    {
        // MainmenuScene에 있는 버튼 이벤트 연결 함수
        OnClickButtons();
        // [PopupPanel] Result 처리 함수
        ShowResultIfAvailable();
    }

    private void Update()
    {
        // 화면 클릭 시 [PopupPanel] Result 닫는 함수 호출
        if (isResultActive && Input.GetMouseButtonDown(0)) CloseResultPanel();
    }
    
    #region 버튼 클릭 이벤트 연결 함수

    private void OnClickButtons()
    {
        // 게임시작 버튼 이벤트 연결 (장면전환: MainmenuScene -> MatchingScene)
        gameStartButton.onClick.AddListener(OnGameStartClicked);
        createServerButton.onClick.AddListener(OnCreateServerClicked);
        rankingButton.onClick.AddListener(OnRankingClicked);
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
        OpenPanel("[PopupPanel] Ranking");
    }

    private void OnGameSettingClicked()
    {
        OpenPanel("[PopupPanel] Settings");
    }

    private void OnGameExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    #endregion

    #region [PopupPanel] Settings Button 처리

    private void OnCloseSettingClicked()
    {
        OpenPanel("[Panel] MainMenu");
    }
    
    #endregion
    
    #region [PopupPanel] Result 처리

    private void ShowResultIfAvailable()
    {
        
        if (GameResultData.totalRounds > 0)
        {
            OpenPanel("[PopupPanel] Result");
            
            // 결과 텍스트 설정
            resultText.text = GameResultData.IsWin ? "승리" : "패배";
            totalRoundText.text = $"{GameResultData.totalRounds}R";
            playerWinText.text = GameResultData.playerWins.ToString();
            opponentWinText.text = GameResultData.opponentWins.ToString();
            
            isResultActive = true;
            
            // 결과 데이터 초기화 (중복 표시 방지)
            ResetGameResultData();
        }
        
    }

    private void ResetGameResultData()
    {
        GameResultData.totalRounds = 0;
        GameResultData.playerWins = 0;
        GameResultData.opponentWins = 0;
    }
    
    #endregion

    private void CloseResultPanel()
    {
        resultPanel.SetActive(false);
        OpenPanel("[Panel] MainMenu");
        isResultActive = false;
    }
}
